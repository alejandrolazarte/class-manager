namespace ClassManager.Infrastructure.Notifications;

internal interface INotificationTexts
{
    string Language { get; }

    string OrderReadyTitle { get; }

    string GuardianConsentBody { get; }

    string StudentAppInvitationDeclinedBody { get; }

    string GuardianConsentRefusedBody { get; }

    string TeamInvitationDeclinedBody { get; }

    string AnnouncementReminderTitle(string announcementTitle);

    string GuardianConsentTitle(string studentFullName);

    string ClassCancelledTitle(string classGroupName);

    string ClassCancelledBody(DateOnly date, TimeOnly startTime, string? reason);

    string ClassChangedTitle(string classGroupName);

    string ClassChangedBody(DateOnly date, TimeOnly startTime, string? substituteFullName);

    string FeedbackTitle(string? instructorFullName, string studentFullName);

    string AbsenceTitle(string studentFullName);

    string MakeupTitle(string studentFullName);

    string PackClassTitle(string studentFullName);

    string ClassBody(string classGroupName, DateOnly date, TimeOnly startTime);

    string StudentAppInvitationDeclinedTitle(string fullName);

    string GuardianConsentRefusedTitle(string fullName);

    string TeamInvitationDeclinedTitle(string email);

    string OrderPlacedTitle(string orderNumber, string clientFullName);
}
