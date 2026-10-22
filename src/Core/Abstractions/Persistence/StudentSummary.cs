namespace ClassManager.Core.Abstractions.Persistence;

public sealed record StudentSummary(
    long Id,
    string FullName,
    DateOnly? BirthDate,
    string? Notes,
    long ClientId,
    string ClientFullName,
    string ClientPhoneNumber);
