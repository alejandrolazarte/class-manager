namespace ClassManager.Core.Abstractions.Persistence;

public sealed record ClientSearchCriteria(
    string? FullNameFragment,
    string? PhoneNumberPrefix,
    int Limit);
