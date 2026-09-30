using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.U.Tests.Domain.Notifications.When_TeamNotification_text_is_too_long;

public sealed class Then_it_is_shortened
{
    [Fact]
    public void Then_it_is_shortened_Run()
    {
        var notification = TeamNotification.Create(
            Guid.CreateVersion7(), new string('t', 500), new string('b', 900), "/today", TestData.Now);

        notification.Title.Length.ShouldBe(TeamNotification.TitleMaxLength);
        notification.Body.Length.ShouldBe(TeamNotification.BodyMaxLength);
    }
}
