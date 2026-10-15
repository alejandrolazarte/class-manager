using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_is_edited;

public sealed class Then_it_shows_the_new_content
{
    [Fact]
    public void Then_it_shows_the_new_content_Run()
    {
        var announcement = Announcement.Create("Feriado", null, TestData.Now, TestData.Today).Value!;

        var result = announcement.Edit(
            "  Clase abierta  ", "Traé a un amigo.", AnnouncementCategory.Event, null, TestData.Today.AddDays(3), TestData.Today);

        result.IsSuccess.ShouldBeTrue();
        announcement.Title.ShouldBe("Clase abierta");
        announcement.Body.ShouldBe("Traé a un amigo.");
        announcement.Category.ShouldBe(AnnouncementCategory.Event);
        announcement.VisibleUntil.ShouldBe(TestData.Today.AddDays(3));
        announcement.PublishedAt.ShouldBe(TestData.Now);
    }
}
