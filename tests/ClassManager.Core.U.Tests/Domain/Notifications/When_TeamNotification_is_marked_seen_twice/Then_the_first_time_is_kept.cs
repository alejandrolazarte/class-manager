using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.U.Tests.Domain.Notifications.When_TeamNotification_is_marked_seen_twice;

public sealed class Then_the_first_time_is_kept
{
    [Fact]
    public void Then_the_first_time_is_kept_Run()
    {
        var notification = TeamNotification.Create(Guid.CreateVersion7(), "Tomás no viene", "Natación inicial", "/today", TestData.Now);

        notification.MarkSeen(TestData.Now.AddMinutes(1));
        notification.MarkSeen(TestData.Now.AddMinutes(5));

        notification.SeenAt.ShouldBe(TestData.Now.AddMinutes(1));
    }
}
