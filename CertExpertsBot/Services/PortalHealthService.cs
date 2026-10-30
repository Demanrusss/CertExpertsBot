using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CertExpertsBot.Services.Models;
using CertExpertsBot.Services.Options;

namespace CertExpertsBot.Services;

public sealed class PortalHealthService
{
    private const string ServiceGroupId = "6d3169a2-dc30-4962-be32-2d1b88b1fa89";
    private const string RegistryApi = "/Synergy/rest/api/registry/";
    private const string CertificationRegistryCode = "registry_certification_conformity_porducts_eaeu";
    private const string CertificationRouteName = "Сервис подтверждения: сертификация ЕАЭС";
    private static readonly HttpClient SharedClient = new(new SocketsHttpHandler
    {
        AutomaticDecompression = DecompressionMethods.All,
        PooledConnectionLifetime = TimeSpan.FromMinutes(5),
        MaxAutomaticRedirections = 5,
        UseCookies = false
    }) { Timeout = Timeout.InfiniteTimeSpan };

    private static readonly Lazy<PortalHealthService> SharedInstance = new(() => new PortalHealthService(SharedClient,
        PortalHealthOptions.FromAppSettings(Path.Combine(AppContext.BaseDirectory, "appsettings.json"))));
    public static PortalHealthService Instance => SharedInstance.Value;

    private readonly HttpClient _client;
    private readonly PortalHealthOptions _options;
    private readonly AuthenticationHeaderValue? _accountAuthorization;
    private readonly bool _accountConfigured;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly SemaphoreSlim _requests = new(3, 3);
    private PortalHealthReport? _cached;

    private PortalHealthService(HttpClient client, PortalHealthOptions? options = null)
    {
        _client = client;
        _options = options ?? new PortalHealthOptions();
        if (_options.CheckTimeout <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(options));
        _accountConfigured = _options.RequireAccount || !string.IsNullOrEmpty(_options.AccountLogin) || !string.IsNullOrEmpty(_options.AccountPassword);
        if (!string.IsNullOrEmpty(_options.AccountLogin) && !string.IsNullOrEmpty(_options.AccountPassword))
            _accountAuthorization = BasicAuthorization(_options.AccountLogin, _options.AccountPassword);
    }

    public async Task<PortalHealthReport> CheckAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (_cached is not null && DateTimeOffset.UtcNow - _cached.CheckedAt < TimeSpan.FromMinutes(1))
                return _cached with { FromCache = true };
            using var budget = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            budget.CancelAfter(_options.CheckTimeout);
            var context = new ProbeContext(cancellationToken, budget.Token);
            AuthenticationHeaderValue? guest = null;
            int? appId = null;
            var resources = await Task.WhenAll(
                ProbeAsync("Страница", "/index/", body =>
                {
                    appId = ReadApplicationId(body);
                    return appId is not null;
                }, context, critical: false),
                ProbeAsync("Скрипт интерфейса", "/constructor/player.js", b => b.Contains("/constructor/rest/"), context, critical: false),
                ProbeAsync("API Synergy", "/Synergy/rest/translation/locales/", IsLocales, context, critical: false));
            var configuration = appId is not null
                ? await ProbeAsync("API приложения", $"/constructor/rest/apps/{appId.Value}", body =>
                {
                    if (!IsApplication(body, appId.Value)) return false;
                    guest = ReadGuestAccess(body);
                    return true;
                }, context)
                : new PortalProbeResult("API приложения", false, TimeSpan.Zero,
                    "страница не сообщила ID приложения", RequestAttempted: false);
            var basic = resources.Append(configuration).ToArray();

            // Prioritize the user's real workflow before synthetic searches and public samples.
            var cabinet = await CheckCabinetAsync(guest, context);
            var certification = await CheckCertificationRouteAsync(cabinet, context);
            var registryTasks = new[]
            {
                CheckRegistryAsync("Сертификаты РК", "certificates_reg_rk", guest, context),
                CheckRegistryAsync("Сертификаты ЕАЭС", "certificates_reg_eaeu", guest, context),
                CheckRegistryAsync("Декларации ЕАЭС", "eaeu_production_conformity_declaration", guest, context)
            };
            var registries = await Task.WhenAll(registryTasks);
            // Bounded, independent search tests: do not retry expensive requests.
            var searches = await Task.WhenAll(
                CheckSearchAsync("Сертификаты РК", "certificates_reg_rk", guest, context),
                CheckSearchAsync("Сертификаты ЕАЭС", "certificates_reg_eaeu", guest, context),
                CheckSearchAsync("Декларации ЕАЭС", "eaeu_production_conformity_declaration", guest, context));
            cancellationToken.ThrowIfCancellationRequested();
            _cached = new PortalHealthReport(DateTimeOffset.UtcNow,
                basic.Concat(cabinet.Probes).Concat(certification.Probes).Concat(registries.SelectMany(r => r.Probes)).Concat(searches).ToArray(),
                new[] { cabinet.Scenario, certification.Scenario }.Concat(registries.Select(r => r.Scenario)).ToArray(), _accountConfigured);
            return _cached;
        }
        finally { _gate.Release(); }
    }

    private async Task<ScenarioCheck> CheckCabinetAsync(AuthenticationHeaderValue? guest, ProbeContext context)
    {
        var probes = new List<PortalProbeResult>();
        var auth = _accountConfigured ? _accountAuthorization : guest;
        if (auth is null)
        {
            probes.Add(new PortalProbeResult("Кабинет: доступ", false, TimeSpan.Zero,
                _accountConfigured ? AccountConfigurationError() : "не удалось получить гостевой доступ из API приложения",
                RequestAttempted: false));
            return Scenario("Загрузка кабинета", probes, TimeSpan.Zero);
        }
        if (_accountConfigured)
        {
            var login = await ProbeAsync("Кабинет: авторизация", "/Synergy/rest/api/person/auth?getGroups=true", body =>
            {
                using var json = JsonDocument.Parse(body);
                var root = json.RootElement;
                return root.ValueKind == JsonValueKind.Object && HasNoApiError(root) &&
                    root.TryGetProperty("person", out var person) && person.ValueKind == JsonValueKind.Object &&
                    root.TryGetProperty("groups", out var groups) && groups.ValueKind == JsonValueKind.Array;
            }, context, auth, 25);
            probes.Add(login);
            if (!login.Success) return Scenario("Загрузка кабинета", probes, login.Elapsed);
        }
        RouteRegistry? route = null;
        var tree = await ProbeAsync("Кабинет: список услуг", RegistryApi + "list?regGroupID=" + ServiceGroupId,
            body =>
            {
                using var json = JsonDocument.Parse(body);
                if (!IsRegistryTree(json.RootElement)) return false;
                if (_accountConfigured) route = FindCertificationRegistry(json.RootElement);
                return true;
            }, context, auth, 25);
        probes.Add(tree);
        // Measure shared dictionaries even if the service-tree call failed, to distinguish bottlenecks.
        var metadata = await Task.WhenAll(
            ProbeAsync("Кабинет: названия групп", RegistryApi + "data_ext?registryCode=registry_portal_settings_registry_groups" +
                "&fields=textbox_reg_group_id&fields=textbox_registry_group_name_ru&fields=textbox_registry_group_name_kz" +
                "&fields=textbox_registry_group_name_en&fields=file_registry_group_icon",
                b => IsDictionary(b, "textbox_reg_group_id"), context, auth, 25),
            ProbeAsync("Кабинет: названия реестров", RegistryApi + "data_ext?registryCode=registry_portal_settings_registries" +
                "&fields=textbox_registry_code&fields=textbox_registry_name_ru&fields=textbox_registry_name_kz&fields=textbox_registry_name_en",
                b => IsDictionary(b, "textbox_registry_code"), context, auth, 25));
        probes.AddRange(metadata);
        var duration = probes.Take(probes.Count - metadata.Length).Aggregate(TimeSpan.Zero, (s, p) => s + p.Elapsed)
            + metadata.Max(p => p.Elapsed);
        return Scenario("Загрузка кабинета", probes, duration) with { CertificationRegistry = route };
    }

    private static ScenarioCheck Scenario(string name, IReadOnlyList<PortalProbeResult> probes, TimeSpan duration) =>
        new(new PortalScenarioResult(name, probes.Where(p => p.Critical).All(p => p.Success), duration, probes.Any(p => p.RequestAttempted)), probes);

    private static RouteRegistry? FindCertificationRegistry(JsonElement node)
    {
        if (node.ValueKind == JsonValueKind.Object)
            return node.TryGetProperty("consistOf", out var children) ? FindCertificationRegistry(children) : null;
        if (node.ValueKind != JsonValueKind.Array) return null;
        foreach (var item in node.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object) continue;
            if (item.TryGetProperty("registryCode", out var code) && code.ValueKind == JsonValueKind.String && code.GetString() == CertificationRegistryCode &&
                item.TryGetProperty("registryID", out var id) && id.ValueKind == JsonValueKind.String && Guid.TryParse(id.GetString(), out _))
            {
                var rights = item.TryGetProperty("rights", out var permissions) && permissions.ValueKind == JsonValueKind.Array
                    ? permissions.EnumerateArray().Where(p => p.ValueKind == JsonValueKind.String).Select(p => p.GetString()!).ToArray() : null;
                // The real menu has four sibling registries. Bound the fan-out if the menu grows.
                var siblings = new[] { CertificationRegistryCode }.Concat(node.EnumerateArray()
                    .Where(p => p.ValueKind == JsonValueKind.Object && p.TryGetProperty("registryCode", out var c) && c.ValueKind == JsonValueKind.String)
                    .Select(p => p.GetProperty("registryCode").GetString()!)).Distinct().Take(8).ToArray();
                return new(id.GetString()!, rights, siblings);
            }
            var found = FindCertificationRegistry(item);
            if (found is not null) return found;
        }
        return null;
    }

    private async Task<ScenarioCheck> CheckCertificationRouteAsync(ScenarioCheck cabinet, ProbeContext context)
    {
        ScenarioCheck Unavailable(string reason) => new(
            new PortalScenarioResult(CertificationRouteName, false, TimeSpan.Zero, RequestAttempted: false, Critical: _accountConfigured),
            [
                new PortalProbeResult("Маршрут ЕАЭС", false, TimeSpan.Zero, reason,
                RequestAttempted: false, Critical: _accountConfigured)
            ]);
        if (!_accountConfigured || _accountAuthorization is null) return Unavailable(AccountConfigurationError());
        var route = cabinet.CertificationRegistry;
        if (route is null) return Unavailable("рабочий реестр не получен из дерева услуг; проверьте вход и права аккаунта");

        var probes = new List<PortalProbeResult>();
        string? firstFilter = null;
        // Opening the last group checks rights/filters for every registry shown in that group.
        var filters = await Task.WhenAll(route.SiblingCodes.Select(code => ProbeAsync(
            code == CertificationRegistryCode ? "Маршрут ЕАЭС: фильтры" : "Маршрут ЕАЭС: права соседнего реестра",
            RegistryApi + "filters?registryCode=" + Uri.EscapeDataString(code), body =>
            {
                using var json = JsonDocument.Parse(body);
                var root = json.RootElement;
                if (root.ValueKind != JsonValueKind.Array || root.EnumerateArray().Any(f => f.ValueKind != JsonValueKind.Object)) return false;
                if (code == CertificationRegistryCode && root.GetArrayLength() > 0 && root[0].TryGetProperty("code", out var filter) && filter.ValueKind == JsonValueKind.String)
                    firstFilter = filter.GetString();
                return true;
            }, context, _accountAuthorization, critical: code == CertificationRegistryCode)));
        probes.AddRange(filters);
        string? formCode = null;
        string[] fields = [];
        var canListWithoutFilter = route.Rights?.Contains("rr_list") == true;
        var info = await ProbeAsync("Маршрут ЕАЭС: структура", RegistryApi + "info?registryID=" + Uri.EscapeDataString(route.Id), body =>
        {
            using var json = JsonDocument.Parse(body);
            var root = json.RootElement;
            if (root.ValueKind != JsonValueKind.Object || !HasNoApiError(root) ||
                !root.TryGetProperty("formCode", out var form) || form.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(form.GetString()) ||
                !root.TryGetProperty("columns", out var columns) || columns.ValueKind != JsonValueKind.Array) return false;
            formCode = form.GetString();
            fields = columns.EnumerateArray().Where(c => c.ValueKind == JsonValueKind.Object &&
                (!c.TryGetProperty("visible", out var visible) || visible.ToString() != "0") &&
                c.TryGetProperty("columnID", out var field) && field.ValueKind == JsonValueKind.String)
                .Select(c => c.GetProperty("columnID").GetString()!).ToArray();
            if (route.Rights is null) canListWithoutFilter = root.TryGetProperty("rr_list", out var listRight) && listRight.ValueKind == JsonValueKind.String && listRight.GetString() == "Y";
            return true;
        }, context, _accountAuthorization);
        probes.Add(info);
        // The UI renders siblings independently (async forEach), so an unrelated
        // filter request is not a blocking dependency of the selected registry.
        var prefix = cabinet.Scenario.Elapsed + filters[0].Elapsed + info.Elapsed;
        if (!info.Success || !filters[0].Success) return Scenario(CertificationRouteName, probes, prefix);

        var listPath = RegistryApi + $"data_ext?registryCode={CertificationRegistryCode}&pageNumber=0&countInPart=15";
        // Same default-filter selection as Registry.showRegistryList in the portal.
        if (!canListWithoutFilter && !string.IsNullOrWhiteSpace(firstFilter)) listPath += "&filterCode=" + Uri.EscapeDataString(firstFilter);
        foreach (var field in fields) listPath += "&fields=" + Uri.EscapeDataString(field);
        long count = 0;
        var pageAndForm = await Task.WhenAll(
            ProbeAsync("Маршрут ЕАЭС: заявки", listPath, body =>
            {
                using var json = JsonDocument.Parse(body);
                return TryReadRegistryPage(json.RootElement, out var rows, out count, 15) && rows.EnumerateArray().All(IsDocumentRow);
            }, context, _accountAuthorization),
            ProbeAsync("Маршрут ЕАЭС: форма фильтров", "/Synergy/rest/api/asforms/form_ext?formCode=" + Uri.EscapeDataString(formCode!), body =>
            {
                using var json = JsonDocument.Parse(body);
                var root = json.RootElement;
                return root.ValueKind == JsonValueKind.Object && HasNoApiError(root) &&
                    root.TryGetProperty("properties", out var properties) && properties.ValueKind == JsonValueKind.Array;
            }, context, _accountAuthorization));
        probes.Add(pageAndForm[0].Success ? pageAndForm[0] with { Details = $"найдено {count:N0}" } : pageAndForm[0]);
        probes.Add(pageAndForm[1]);
        var result = Scenario(CertificationRouteName, probes, prefix + pageAndForm.Max(p => p.Elapsed));
        return result with { Scenario = result.Scenario with { Success = result.Scenario.Success && cabinet.Scenario.Success } };
    }

    private string AccountConfigurationError() => _options.ConfigurationError ??
        "неполная настройка учётной записи: appsettings.json → PortalHealth → AccountLogin / AccountPassword";

    private static bool IsRegistryTree(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Array)
            return root.GetArrayLength() > 0 && root.EnumerateArray().All(IsRegistryTree);
        if (root.ValueKind != JsonValueKind.Object || !HasNoApiError(root)) return false;
        if (root.TryGetProperty("registryCode", out var code) && code.ValueKind == JsonValueKind.String &&
            !string.IsNullOrWhiteSpace(code.GetString()) && root.TryGetProperty("registryID", out var id) && id.ValueKind == JsonValueKind.String)
            return true;
        return root.TryGetProperty("regGroupID", out var group) && group.ValueKind == JsonValueKind.String &&
            root.TryGetProperty("consistOf", out var children) && children.ValueKind == JsonValueKind.Array &&
            children.EnumerateArray().All(IsRegistryTree);
    }

    private static bool IsDictionary(string body, string field)
    {
        using var json = JsonDocument.Parse(body);
        var root = json.RootElement;
        return TryReadRegistryPage(root, out var rows, out var count, int.MaxValue) && count > 0 &&
            // The portal skips unconfigured dictionary rows; some have no registry code.
            // Require a usable mapping, rather than rejecting the whole API for such rows.
            rows.EnumerateArray().Any(row => row.ValueKind == JsonValueKind.Object &&
                (HasDictionaryKey(row, "fieldValue", field) || HasDictionaryKey(row, "fieldKey", field)));
    }

    private static bool HasDictionaryKey(JsonElement row, string property, string field) =>
        row.TryGetProperty(property, out var fields) && fields.ValueKind == JsonValueKind.Object &&
        fields.TryGetProperty(field, out var value) && value.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(value.GetString());

    private async Task<PortalProbeResult> CheckSearchAsync(string name, string code,
        AuthenticationHeaderValue? guest, ProbeContext context)
    {
        if (guest is null) return new(name + ": поиск", false, TimeSpan.Zero,
            "не удалось получить гостевой доступ из API приложения", RequestAttempted: false, Critical: false);
        long count = 0;
        var result = await ProbeAsync(name + ": поиск", RegistryApi + "data_ext?" +
            $"registryCode={code}&pageNumber=0&countInPart=20&filterCode=deistvuuschie&searchString=KZ.00000000.PROBE", body =>
            {
                using var json = JsonDocument.Parse(body);
                return TryReadRegistryPage(json.RootElement, out var rows, out count) && rows.EnumerateArray().All(IsDocumentRow);
            }, context, guest, 25, critical: false);
        return result.Success ? result with { Details = $"найдено {count:N0}" } : result;
    }

    private static bool HasNoApiError(JsonElement root) =>
        !root.TryGetProperty("errorCode", out var error) || error.ToString() == "0";

    private static bool TryReadRegistryPage(JsonElement root, out JsonElement rows, out long count, int pageSize = 20)
    {
        rows = default;
        count = 0;
        return root.ValueKind == JsonValueKind.Object && HasNoApiError(root) &&
            root.TryGetProperty("recordsCount", out var total) && total.ValueKind == JsonValueKind.Number &&
            total.TryGetInt64(out count) && count >= 0 &&
            root.TryGetProperty("result", out rows) && rows.ValueKind == JsonValueKind.Array &&
            rows.GetArrayLength() == Math.Min(count, pageSize);
    }

    private static bool IsDocumentRow(JsonElement row) =>
        row.ValueKind == JsonValueKind.Object &&
        row.TryGetProperty("documentID", out var id) && id.ValueKind == JsonValueKind.String && Guid.TryParse(id.GetString(), out _) &&
        row.TryGetProperty("dataUUID", out var data) && data.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(data.GetString()) &&
        row.TryGetProperty("fieldValue", out var fields) && fields.ValueKind == JsonValueKind.Object;

    private static AuthenticationHeaderValue BasicAuthorization(string login, string password) =>
        new("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}")));

    private static AuthenticationHeaderValue? ReadGuestAccess(string body)
    {
        using var json = JsonDocument.Parse(body);
        var root = json.RootElement;
        if (!root.TryGetProperty("shouldAuth", out var shouldAuth) || shouldAuth.ValueKind != JsonValueKind.False ||
            !root.TryGetProperty("constructorImage", out var image) || image.ValueKind != JsonValueKind.String) return null;
        var parts = image.GetString()!.Split('#', 2);
        if (parts.Length != 2) return null;
        try
        {
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(parts[1])).Split(';');
            return credentials.Length >= 2 && !string.IsNullOrEmpty(credentials[0]) && !string.IsNullOrEmpty(credentials[1])
                ? BasicAuthorization(credentials[0], credentials[1]) : null;
        }
        catch (FormatException) { return null; }
    }

    private async Task<ScenarioCheck> CheckRegistryAsync(string name, string code,
        AuthenticationHeaderValue? guest, ProbeContext context)
    {
        var probes = new List<PortalProbeResult>();
        if (guest is null)
        {
            probes.Add(new PortalProbeResult(name, false, TimeSpan.Zero,
                "не удалось получить гостевой доступ из API приложения", RequestAttempted: false));
            return Scenario(name, probes, TimeSpan.Zero);
        }
        var filters = await ProbeAsync(name + ": фильтры", RegistryApi + "filters?registryCode=" + code, body =>
        {
            using var json = JsonDocument.Parse(body);
            return json.RootElement.ValueKind == JsonValueKind.Array;
        }, context, guest);
        probes.Add(filters);
        var info = await ProbeAsync(name + ": структура", RegistryApi + "info?code=" + code + "&locale=ru", body =>
        {
            using var json = JsonDocument.Parse(body);
            var root = json.RootElement;
            return root.ValueKind == JsonValueKind.Object && HasNoApiError(root) &&
                root.TryGetProperty("columns", out var columns) && columns.ValueKind == JsonValueKind.Array &&
                root.TryGetProperty("formCode", out var form) && form.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(form.GetString());
        }, context, guest);
        probes.Add(info);
        string? documentId = null, dataId = null;
        long count = 0;
        var received = 0;
        var list = await ProbeAsync(name + ": выборка", RegistryApi + "data_ext?" +
            $"registryCode={code}&pageNumber=0&countInPart=20&filterCode=deistvuuschie", body =>
            {
                using var json = JsonDocument.Parse(body);
                if (!TryReadRegistryPage(json.RootElement, out var rows, out count) || count < 1 || !rows.EnumerateArray().All(IsDocumentRow)) return false;
                received = rows.GetArrayLength();
                documentId = rows[0].GetProperty("documentID").GetString();
                dataId = rows[0].GetProperty("dataUUID").GetString();
                return true;
            }, context, guest);
        probes.Add(list.Success ? list with { Details = $"получено {received} из {count:N0}" } : list);
        if (list.Success)
        {
            var documents = await Task.WhenAll(
                ProbeAsync(name + ": свойства", "/Synergy/rest/api/docflow/doc/document_info?documentID=" + Uri.EscapeDataString(documentId!), body =>
                {
                    using var json = JsonDocument.Parse(body);
                    var root = json.RootElement;
                    return root.ValueKind == JsonValueKind.Object && HasNoApiError(root) &&
                        root.TryGetProperty("documentID", out var id) && id.ValueKind == JsonValueKind.String && id.GetString() == documentId &&
                        root.TryGetProperty("asfDataID", out var data) && data.ValueKind == JsonValueKind.String && data.GetString() == dataId &&
                        root.TryGetProperty("registryName", out var registry) && registry.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(registry.GetString());
                }, context, guest),
                ProbeAsync(name + ": содержимое", "/Synergy/rest/api/asforms/data/" + Uri.EscapeDataString(dataId!), body =>
                {
                    using var json = JsonDocument.Parse(body);
                    var root = json.RootElement;
                    return root.ValueKind == JsonValueKind.Object && HasNoApiError(root) &&
                        root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array && data.GetArrayLength() > 0;
                }, context, guest));
            probes.AddRange(documents);
            return Scenario(name, probes, filters.Elapsed + info.Elapsed + list.Elapsed + documents.Max(p => p.Elapsed));
        }
        return Scenario(name, probes, filters.Elapsed + info.Elapsed + list.Elapsed);
    }

    private static int? ReadApplicationId(string body)
    {
        var match = Regex.Match(body, """\bdata-app-id\s*=\s*["']?(?<id>[1-9]\d*)(?=["'\s>])""",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
        return match.Success && int.TryParse(match.Groups["id"].Value, out var id) ? id : null;
    }

    private static bool IsApplication(string body, int expectedId)
    {
        using var json = JsonDocument.Parse(body);
        return json.RootElement.ValueKind == JsonValueKind.Object &&
            json.RootElement.TryGetProperty("id", out var id) && id.ValueKind == JsonValueKind.Number && id.TryGetInt32(out var value) && value == expectedId &&
            json.RootElement.TryGetProperty("content", out _);
    }

    private static bool IsLocales(string body)
    {
        using var json = JsonDocument.Parse(body);
        return json.RootElement.ValueKind == JsonValueKind.Array && json.RootElement.GetArrayLength() > 0 &&
            json.RootElement[0].ValueKind == JsonValueKind.Object && json.RootElement[0].TryGetProperty("localeID", out _);
    }

    private async Task<PortalProbeResult> ProbeAsync(string name, string path, Func<string, bool> validate,
        ProbeContext context, AuthenticationHeaderValue? authorization = null, int timeoutSeconds = 10, bool critical = true)
    {
        var watch = new Stopwatch();
        var entered = false;
        var attempted = false;
        try
        {
            await _requests.WaitAsync(context.BudgetToken);
            entered = true;
            // Queueing in our bounded client is not portal latency.
            watch.Start();
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(context.BudgetToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
            using var request = new HttpRequestMessage(HttpMethod.Get, "https://techreg.gov.kz" + path);
            request.Headers.UserAgent.ParseAdd("CertExpertsBot/1.0");
            request.Headers.Authorization = authorization;
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            context.BudgetToken.ThrowIfCancellationRequested();
            attempted = true;
            using var response =
                await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeout.Token);
            if (!response.IsSuccessStatusCode)
                return new PortalProbeResult(name, false, watch.Elapsed, $"HTTP {(int)response.StatusCode}", Critical: critical);
            await using var stream = await response.Content.ReadAsStreamAsync(timeout.Token);
            using var buffer = new MemoryStream();
            var chunk = new byte[16384];
            int read;
            while ((read = await stream.ReadAsync(chunk, timeout.Token)) > 0)
            {
                if (buffer.Length + read > 8 * 1024 * 1024)
                    return new PortalProbeResult(name, false, watch.Elapsed, "слишком большой ответ", Critical: critical);
                
                buffer.Write(chunk, 0, read);
            }

            buffer.Position = 0;
            using var reader = new StreamReader(buffer);
            var body = await reader.ReadToEndAsync(timeout.Token);
            return validate(body)
                ? new PortalProbeResult(name, true, watch.Elapsed, null, Critical: critical)
                : new PortalProbeResult(name, false, watch.Elapsed, "неожиданный ответ", Critical: critical);
        }
        catch (OperationCanceledException) when (!context.CallerToken.IsCancellationRequested)
        {
            return new PortalProbeResult(name, false, watch.Elapsed, context.BudgetToken.IsCancellationRequested
                ? $"общий лимит проверки {_options.CheckTimeout.TotalSeconds:0} с"
                : $"таймаут {timeoutSeconds} с", RequestAttempted: attempted, Critical: critical);
        }
        catch (HttpRequestException ex)
        {
            return new PortalProbeResult(name, false, watch.Elapsed, DescribeTransportError(ex),
                RequestAttempted: attempted, Critical: critical);
        }
        catch (IOException)
        {
            return new PortalProbeResult(name, false, watch.Elapsed, "обрыв загрузки", Critical: critical);
        }
        catch (JsonException)
        {
            return new PortalProbeResult(name, false, watch.Elapsed, "некорректный JSON", Critical: critical);
        }
        finally
        {
            if (entered)
                _requests.Release();
        }
    }

    private static string DescribeTransportError(HttpRequestException error)
    {
        SocketException? socket = null;
        for (Exception? current = error; current is not null; current = current.InnerException)
            if (current is SocketException found) { socket = found; break; }
        var code = socket is null ? "" : $" ({socket.SocketErrorCode})";
        // Never expose exception messages: they can contain proxy credentials or URLs with secrets.
        return error.HttpRequestError switch
        {
            HttpRequestError.NameResolutionError => "DNS: не удалось определить адрес портала" + code,
            HttpRequestError.SecureConnectionError => "TLS: не удалось установить защищённое соединение",
            HttpRequestError.ConnectionError => "Соединение: не удалось подключиться к порталу" + code,
            HttpRequestError.ProxyTunnelError => "Соединение: ошибка прокси-туннеля",
            _ => $"HTTP-транспорт: {error.HttpRequestError}" + code
        };
    }
}
