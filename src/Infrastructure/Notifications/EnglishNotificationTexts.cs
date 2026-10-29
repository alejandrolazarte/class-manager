using System.Globalization;
using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Infrastructure.Notifications;

internal sealed class EnglishNotificationTexts : INotificationTexts
{
    private const string TimeFormat = "HH:mm";
    private const string DateFormat = "dddd, MMM d";
    private const string ShortDateFormat = "ddd MMM d";

    private static readonly CultureInfo EnglishCulture = CultureInfo.GetCultureInfo("en-US");

    public string Language => AccountLanguage.English;

    public string OrderReadyTitle => "Your order is ready";

    public string GuardianConsentBody => "Open the app to authorize their access or not.";

    public string PhotoApprovalBody => "Open the app to approve it or not.";

    public string StudentAppInvitationDeclinedBody => "If it was a mistake, you can invite them again from their profile.";

    public string GuardianConsentRefusedBody => "Their guardian didn't give authorization. Their classes are still visible from the guardian's account.";

    public string TeamInvitationDeclinedBody => "If it was a mistake, you can invite them again from Team.";

    public string AnnouncementReminderTitle(string announcementTitle) => $"Reminder: {announcementTitle}";

    public string GuardianConsentTitle(string studentFullName) => $"{PersonNames.FirstNameOf(studentFullName)} wants to use the app";

    public string PhotoApprovalTitle(string studentFullName) => $"{PersonNames.FirstNameOf(studentFullName)} wants to add a profile photo";

    public string ClassCancelledTitle(string classGroupName) => $"{classGroupName} is cancelled";

    public string ClassCancelledBody(DateOnly date, TimeOnly startTime, string? reason)
    {
        var when = $"There's no class on {Date(date)} at {Time(startTime)}.";
        return string.IsNullOrWhiteSpace(reason) ? when : $"{when} {reason}";
    }

    public string ClassChangedTitle(string classGroupName) => $"Change in {classGroupName}";

    public string ClassChangedBody(DateOnly date, TimeOnly startTime, string? substituteFullName)
    {
        var when = $"The class on {Date(date)} is at {Time(startTime)}";
        return substituteFullName is null ? $"{when}." : $"{when} with {substituteFullName}.";
    }

    public string FeedbackTitle(string? instructorFullName, string studentFullName)
    {
        var studentFirstName = PersonNames.FirstNameOf(studentFullName);
        return instructorFullName is null
            ? $"A comment on {studentFirstName}'s class"
            : $"{instructorFullName} commented on {studentFirstName}'s class";
    }

    public string AbsenceTitle(string studentFullName) => $"{PersonNames.FirstNameOf(studentFullName)} said they're not coming";

    public string MakeupTitle(string studentFullName) => $"{PersonNames.FirstNameOf(studentFullName)} is coming to make up a class";

    public string PackClassTitle(string studentFullName) => $"{PersonNames.FirstNameOf(studentFullName)} booked a pack class";

    public string ClassBody(string classGroupName, DateOnly date, TimeOnly startTime) =>
        $"{classGroupName} · {date.ToString(ShortDateFormat, EnglishCulture)} {Time(startTime)}";

    public string StudentAppInvitationDeclinedTitle(string fullName) => $"{PersonNames.FirstNameOf(fullName)} declined the app invitation";

    public string GuardianConsentRefusedTitle(string fullName) => $"{PersonNames.FirstNameOf(fullName)} wasn't authorized to use the app";

    public string TeamInvitationDeclinedTitle(string email) => $"{email} declined the team invitation";

    public string OrderPlacedTitle(string orderNumber, string clientFullName) => $"New order {orderNumber} from {clientFullName}";

    private static string Date(DateOnly date) => date.ToString(DateFormat, EnglishCulture);

    private static string Time(TimeOnly time) => time.ToString(TimeFormat, CultureInfo.InvariantCulture);
}
