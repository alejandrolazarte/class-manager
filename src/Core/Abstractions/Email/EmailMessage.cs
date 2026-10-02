using ClassManager.Notifications.Email;

namespace ClassManager.Core.Abstractions.Email;

public sealed record EmailMessage(string To, string Subject, EmailContent Content, Guid? BusinessId = null)
{
    public string TextBody => Content.ToPlainText();
}
