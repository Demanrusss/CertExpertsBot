using System.Text;

namespace CertExpertsBot.Services.Models;

public sealed record PortalHealthReport(
    DateTimeOffset CheckedAt,
    IReadOnlyList<PortalProbeResult> Probes,
    IReadOnlyList<PortalScenarioResult>? Scenarios = null,
    bool AccountConfigured = false,
    bool FromCache = false)
{
    public string ToMessage()
    {
        var scenarios = Scenarios ?? [];
        var attempted = Probes.Where(p => p.RequestAttempted).ToArray();
        var skipped = Probes.Where(p => !p.RequestAttempted).ToArray();
        var slowest = attempted.MaxBy(p => p.Elapsed);
        var maxSeconds = Math.Max(attempted.Where(p => p.Critical).Select(p => p.Elapsed.TotalSeconds).DefaultIfEmpty().Max(),
            scenarios.Where(s => s.Critical).Select(s => s.Elapsed.TotalSeconds).DefaultIfEmpty().Max());
        var diagnosticIssues = attempted.Any(p => !p.Critical && (!p.Success || p.Elapsed.TotalSeconds >= 3));
        var verdict = attempted.Length == 0 || Probes.Any(p => p.Critical && !p.Success) || scenarios.Any(s => s.Critical && !s.Success)
            ? "🔴 Лучше подождать: часть проверок не прошла."
            : maxSeconds >= 10
                ? "🔴 Лучше подождать: запросы или их каскад отвечают очень медленно."
                : maxSeconds >= 3 || diagnosticIssues
                    ? "🟡 Основные сервисы доступны; работать можно, но есть задержки или замечания."
                    : "🟢 Можно пробовать работать: проверенные сервисы отвечают быстро.";
        var sb = new StringBuilder($"Портал techreg.gov.kz\n{verdict}\n");
        sb.AppendLine(FromCache
            ? $"Источник: кэш, возраст {Math.Max(0, (int)(DateTimeOffset.UtcNow - CheckedAt).TotalSeconds)} с; повторных запросов не было."
            : "Источник: новая проверка.");
        sb.AppendLine(AccountConfigured ? "Кабинет: профиль учётной записи." : "Кабинет: гостевой профиль; вход пользователя не проверен.");
        if (scenarios.Count > 0)
        {
            sb.AppendLine("\nКаскады API (расчёт по шагам):");
            foreach (var s in scenarios)
                sb.AppendLine(s.RequestAttempted
                    ? $"{(s.Success ? "✓" : "✗")} {s.Name}: {FormatElapsed(s.Elapsed)}{(s.Success ? "" : " — не завершён") }"
                    : $"— {s.Name}: не проверен");
        }
        sb.AppendLine($"\nПопыток HTTP-запросов: {attempted.Length}. Успешно: {attempted.Count(p => p.Success)}/{attempted.Length}. Пропущено: {skipped.Length}.");
        var failures = attempted.Where(p => !p.Success).ToArray();
        foreach (var p in failures.Take(6))
            sb.AppendLine($"✗ {p.Name}{(p.Critical ? "" : " (доп. проверка)")}: {p.Error} ({FormatElapsed(p.Elapsed)})");
        if (failures.Length > 6) sb.AppendLine($"Ещё неуспешных проверок: {failures.Length - 6}.");
        if (diagnosticIssues) sb.AppendLine("Тестовый поиск и ресурсы интерфейса учитываются как дополнительные проверки; их задержки сами по себе не означают отказ рабочих разделов.");
        foreach (var p in skipped.Take(2)) sb.AppendLine($"— {p.Name}: пропущено — {p.Error}.");
        if (slowest is not null)
        {
            sb.AppendLine("Самые медленные запросы:");
            foreach (var p in attempted.OrderByDescending(p => p.Elapsed).Take(5))
                sb.AppendLine($"{(p.Success ? "✓" : "✗")} {p.Name}: {FormatElapsed(p.Elapsed)}{(p.Details is null ? "" : $" — {p.Details}")}");
            sb.AppendLine($"Дольше всего: {slowest.Name} — {FormatElapsed(slowest.Elapsed)}.");
        }
        if (failures.Length > 0 && failures.All(p => p.Error?.StartsWith("DNS:") == true ||
            p.Error?.StartsWith("TLS:") == true || p.Error?.StartsWith("Соединение:") == true || p.Error?.StartsWith("HTTP-транспорт:") == true))
            sb.AppendLine("Сервер бота не смог подключиться к порталу. Это не измерение скорости его API.");
        sb.AppendLine($"\nПроверено: {CheckedAt.ToOffset(TimeSpan.FromHours(5)):dd.MM.yyyy HH:mm:ss} (UTC+5). Кэш до 1 минуты.");
        sb.Append("Замер с сервера бота. Каскад — оценка сетевых шагов без кэша браузера и отрисовки. ЭЦП, запись документов и работа самой БД не проверяются.");
        return sb.ToString();
    }

    private static string FormatElapsed(TimeSpan value) => value.TotalSeconds < 1
        ? $"{Math.Max(1, Math.Ceiling(value.TotalMilliseconds)):0} мс" : $"{value.TotalSeconds:F1} с";
}