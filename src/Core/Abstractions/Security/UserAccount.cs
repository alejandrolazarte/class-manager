namespace ClassManager.Core.Abstractions.Security;

public sealed record UserAccount(Guid UserId, string Email, string FullName);
