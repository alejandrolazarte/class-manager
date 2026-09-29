using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.Abstractions.Persistence;

public sealed record ClientSearchCriteria(
    string? FullNameFragment,
    string? PhoneNumberPrefix,
    int Limit,
    ClientScope? Scope = null);
