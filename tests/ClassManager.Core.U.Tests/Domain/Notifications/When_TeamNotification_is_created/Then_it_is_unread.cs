using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.U.Tests.Domain.Notifications.When_TeamNotification_is_created;

public sealed class Then_it_is_unread
{
    [Fact]
    public void Then_it_is_unread_Run()
    {
        var notification = TeamNotification.Create(Guid.CreateVersion7(), "Tomás no viene", "Natación inicial · Mié 1/10", "/today", TestData.Now);

        notification.SeenAt.ShouldBeNull();
    }
}
