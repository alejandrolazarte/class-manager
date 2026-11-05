using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.U.Tests.Domain.Notifications.When_NotificationCategories_are_listed_for_a_family;

public sealed class Then_only_family_categories_are_listed
{
    [Fact]
    public void Then_only_family_categories_are_listed_Run()
    {
        var categories = NotificationCategories.For(NotificationAudience.Family);

        categories.Select(definition => definition.Audience).Distinct().ShouldBe([NotificationAudience.Family]);
    }
}
