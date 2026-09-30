using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_is_published;

public sealed class Then_its_texts_are_trimmed
{
    [Fact]
    public void Then_its_texts_are_trimmed_Run()
    {
        var announcement = Announcement.Create("  Lunes cerrado ", "  Feriado nacional.  ", TestData.Now).Value!;

        announcement.Title.ShouldBe("Lunes cerrado");
        announcement.Body.ShouldBe("Feriado nacional.");
        announcement.PublishedAt.ShouldBe(TestData.Now);
    }
}
