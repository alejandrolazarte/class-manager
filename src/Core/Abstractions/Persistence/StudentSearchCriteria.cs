using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.Abstractions.Persistence;

public sealed record StudentSearchCriteria(
    string? FullNameFragment,
    string? ClientPhoneNumberPrefix,
    int Limit,
    ClientScope? Scope = null);
