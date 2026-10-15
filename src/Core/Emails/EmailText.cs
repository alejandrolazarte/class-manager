using ClassManager.Notifications.Email;

namespace ClassManager.Core.Emails;

public sealed record EmailText(string Subject, EmailContent Content);
