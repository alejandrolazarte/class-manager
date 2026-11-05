using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.U.Tests.Domain.Notifications.When_NotificationCategories_are_listed;

public sealed class Then_every_category_is_listed_once
{
    [Fact]
    public void Then_every_category_is_listed_once_Run()
    {
        var categories = NotificationCategories.All.Select(definition => definition.Category).ToList();

        categories.ShouldBe(Enum.GetValues<NotificationCategory>(), ignoreOrder: true);
    }
}
