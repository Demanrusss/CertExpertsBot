namespace CertExpertsBot.Services.Models;

public sealed record PortalScenarioResult(
    string Name,
    bool Success,
    TimeSpan Elapsed,
    bool RequestAttempted = true,
    bool Critical = true);