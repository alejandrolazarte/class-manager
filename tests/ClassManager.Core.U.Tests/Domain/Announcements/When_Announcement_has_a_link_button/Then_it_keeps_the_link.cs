using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_has_a_link_button;

public sealed class Then_it_keeps_the_link
{
    [Fact]
    public void Then_it_keeps_the_link_Run()
    {
        var announcement = Announcement.Create(
            "Inscripciones abiertas", null, TestData.Now, TestData.Today,
            action: AnnouncementAction.Link, actionUrl: "  https://example.com/inscripcion  ").Value!;

        announcement.Action.ShouldBe(AnnouncementAction.Link);
        announcement.ActionUrl.ShouldBe("https://example.com/inscripcion");
    }
}
