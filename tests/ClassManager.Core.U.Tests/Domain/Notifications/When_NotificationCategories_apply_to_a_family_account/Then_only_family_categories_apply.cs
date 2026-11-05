using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.U.Tests.Domain.Notifications.When_NotificationCategories_apply_to_a_family_account;

public sealed class Then_only_family_categories_apply
{
    [Fact]
    public void Then_only_family_categories_apply_Run()
    {
        var categories = NotificationCategories.ApplyingTo(isTeamMember: false, hasFamilyAccess: true);

        categories.Select(definition => definition.Audience).Distinct().ShouldBe([NotificationAudience.Family]);
    }
}
