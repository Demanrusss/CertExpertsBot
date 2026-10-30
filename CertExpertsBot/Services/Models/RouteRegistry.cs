namespace CertExpertsBot.Services.Models;

public sealed record RouteRegistry(
    string Id,
    string[]? Rights,
    string[] SiblingCodes);