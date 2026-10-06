namespace ClassManager.Core.Abstractions.Security;

public sealed record EmailChangeRequested(string Token, string CurrentEmail);

public sealed record ConfirmedEmailChange(Guid UserId, string PreviousEmail, string NewEmail);
