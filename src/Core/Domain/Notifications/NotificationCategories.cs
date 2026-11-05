namespace ClassManager.Core.Domain.Notifications;

public static class NotificationCategories
{
    private static readonly NotificationChannel[] EmailAndPush = [NotificationChannel.Email, NotificationChannel.Push];
    private static readonly NotificationChannel[] PushOnly = [NotificationChannel.Push];

    public static IReadOnlyList<NotificationCategoryDefinition> All { get; } =
    [
        new(NotificationCategory.Orders, NotificationAudience.Family, EmailAndPush),
        new(NotificationCategory.News, NotificationAudience.Family, PushOnly),
        new(NotificationCategory.ClassChanges, NotificationAudience.Family, PushOnly),
        new(NotificationCategory.InstructorComments, NotificationAudience.Family, PushOnly),
        new(NotificationCategory.NewOrders, NotificationAudience.Team, EmailAndPush),
        new(NotificationCategory.Attendance, NotificationAudience.Team, PushOnly),
        new(NotificationCategory.StudentPlans, NotificationAudience.Team, PushOnly),
        new(NotificationCategory.Invitations, NotificationAudience.Team, PushOnly),
    ];

    public static NotificationCategoryDefinition Of(NotificationCategory category) =>
        All.First(definition => definition.Category == category);

    public static IReadOnlyList<NotificationCategoryDefinition> ApplyingTo(bool isTeamMember, bool hasFamilyAccess) =>
    [
        .. All.Where(definition => definition.Audience switch
        {
            NotificationAudience.Team => isTeamMember,
            _ => hasFamilyAccess,
        }),
    ];
}
