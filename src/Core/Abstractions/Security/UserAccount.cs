using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.Abstractions.Security;

public sealed record UserAccount(
    Guid UserId,
    string Email,
    string FullName,
    DateOnly? BirthDate = null,
    string PreferredLanguage = AccountLanguage.Default);
