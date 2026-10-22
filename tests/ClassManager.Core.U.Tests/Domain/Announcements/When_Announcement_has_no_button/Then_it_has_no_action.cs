using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_has_no_button;

public sealed class Then_it_has_no_action
{
    [Fact]
    public void Then_it_has_no_action_Run()
    {
        var announcement = Announcement.Create("Feriado", null, TestData.Now, TestData.Today).Value!.WithId();

        announcement.Action.ShouldBe(AnnouncementAction.None);
        announcement.ActionUrl.ShouldBeNull();
    }
}
