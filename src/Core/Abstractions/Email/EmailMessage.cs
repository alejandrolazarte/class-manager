namespace ClassManager.Core.Abstractions.Email;

public sealed record EmailMessage(string To, string Subject, string TextBody);
