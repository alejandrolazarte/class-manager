using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_link_button_has_no_link;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var announcement = Announcement.Create(
            "Inscripciones abiertas", null, TestData.Now, TestData.Today, action: AnnouncementAction.Link);

        announcement.Error!.FieldName.ShouldBe(nameof(Announcement.ActionUrl));
    }
}
