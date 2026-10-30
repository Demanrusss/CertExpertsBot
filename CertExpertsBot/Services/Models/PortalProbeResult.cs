namespace CertExpertsBot.Services.Models;

public sealed record PortalProbeResult(
    string Name,
    bool Success,
    TimeSpan Elapsed,
    string? Error,
    string? Details = null,
    bool RequestAttempted = true,
    bool Critical = true);