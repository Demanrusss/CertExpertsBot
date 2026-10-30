namespace CertExpertsBot.Services.Models;

public sealed record ScenarioCheck(
    PortalScenarioResult Scenario,
    IReadOnlyList<PortalProbeResult> Probes,
    RouteRegistry? CertificationRegistry = null);