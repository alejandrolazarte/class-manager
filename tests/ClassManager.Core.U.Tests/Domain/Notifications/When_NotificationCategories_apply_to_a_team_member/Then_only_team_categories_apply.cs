using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.U.Tests.Domain.Notifications.When_NotificationCategories_apply_to_a_team_member;

public sealed class Then_only_team_categories_apply
{
    [Fact]
    public void Then_only_team_categories_apply_Run()
    {
        var categories = NotificationCategories.ApplyingTo(isTeamMember: true, hasFamilyAccess: false);

        categories.Select(definition => definition.Audience).Distinct().ShouldBe([NotificationAudience.Team]);
    }
}
