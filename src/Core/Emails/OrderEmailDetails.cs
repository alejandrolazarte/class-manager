using ClassManager.Notifications.Email;

namespace ClassManager.Core.Emails;

public sealed record OrderEmailDetails(int Number, string BusinessName, IReadOnlyList<EmailLine> Lines, string Total);
