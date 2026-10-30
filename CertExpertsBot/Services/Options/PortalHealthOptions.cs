using System.Text.Json;

namespace CertExpertsBot.Services.Options;

public sealed class PortalHealthOptions
{
    public string? AccountLogin { get; init; }
    public string? AccountPassword { get; init; }
    public TimeSpan CheckTimeout { get; init; } = TimeSpan.FromSeconds(90);
    public bool RequireAccount { get; init; }
    public string? ConfigurationError { get; init; }

    public static PortalHealthOptions FromAppSettings(string path)
    {
        try
        {
            using var json = JsonDocument.Parse(File.ReadAllText(path));
            if (json.RootElement.ValueKind != JsonValueKind.Object ||
                !json.RootElement.TryGetProperty("PortalHealth", out var section) || section.ValueKind != JsonValueKind.Object)
                return new PortalHealthOptions { RequireAccount = true, ConfigurationError = "в appsettings.json отсутствует раздел PortalHealth" };
            var settings = section.Deserialize<PortalHealthOptions>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (settings?.CheckTimeout <= TimeSpan.Zero)
                return new PortalHealthOptions { RequireAccount = true, ConfigurationError = "некорректный CheckTimeout в разделе PortalHealth файла appsettings.json" };
            return new PortalHealthOptions
            {
                AccountLogin = settings?.AccountLogin,
                AccountPassword = settings?.AccountPassword,
                CheckTimeout = settings?.CheckTimeout ?? TimeSpan.FromSeconds(90),
                // The production bot must never silently substitute the guest profile.
                RequireAccount = true
            };
        }
        catch (JsonException) { return new() { RequireAccount = true, ConfigurationError = "некорректный раздел PortalHealth в appsettings.json" }; }
        catch (IOException) { return new() { RequireAccount = true, ConfigurationError = "не удалось прочитать appsettings.json рядом с ботом" }; }
        catch (UnauthorizedAccessException) { return new() { RequireAccount = true, ConfigurationError = "нет доступа к appsettings.json рядом с ботом" }; }
    }
}