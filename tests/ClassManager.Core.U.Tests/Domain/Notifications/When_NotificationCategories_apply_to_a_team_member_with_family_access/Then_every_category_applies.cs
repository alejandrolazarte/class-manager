using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.U.Tests.Domain.Notifications.When_NotificationCategories_apply_to_a_team_member_with_family_access;

public sealed class Then_every_category_applies
{
    [Fact]
    public void Then_every_category_applies_Run()
    {
        var categories = NotificationCategories.ApplyingTo(isTeamMember: true, hasFamilyAccess: true);

        categories.ShouldBe(NotificationCategories.All);
    }
}
