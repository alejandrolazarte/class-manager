namespace ClassManager.Core.Abstractions.Persistence;

public sealed record StudentSummary(
    Guid Id,
    string FullName,
    DateOnly? BirthDate,
    string? Notes,
    Guid ClientId,
    string ClientFullName,
    string ClientPhoneNumber);
