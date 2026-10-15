using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_is_edited_to_hide_before_today;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var announcement = Announcement.Create("Feriado", null, TestData.Now, TestData.Today).Value!;

        var result = announcement.Edit("Feriado", null, AnnouncementCategory.News, null, TestData.Today.AddDays(-1), TestData.Today);

        result.Error!.FieldName.ShouldBe(nameof(Announcement.VisibleUntil));
    }
}
