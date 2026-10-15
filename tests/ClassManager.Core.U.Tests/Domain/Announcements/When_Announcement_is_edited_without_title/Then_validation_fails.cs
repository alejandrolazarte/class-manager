using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_is_edited_without_title;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var announcement = Announcement.Create("Feriado", null, TestData.Now, TestData.Today).Value!;

        var result = announcement.Edit(" ", null, AnnouncementCategory.News, null, null, TestData.Today);

        result.Error!.FieldName.ShouldBe(nameof(Announcement.Title));
        announcement.Title.ShouldBe("Feriado");
    }
}
