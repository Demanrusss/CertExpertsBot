namespace CertExpertsBot.Services.Models;

public sealed record ProbeContext(
    CancellationToken CallerToken,
    CancellationToken BudgetToken);