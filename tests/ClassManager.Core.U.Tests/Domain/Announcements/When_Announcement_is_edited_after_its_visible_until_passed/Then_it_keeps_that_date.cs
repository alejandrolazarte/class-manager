using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_is_edited_after_its_visible_until_passed;

public sealed class Then_it_keeps_that_date
{
    [Fact]
    public void Then_it_keeps_that_date_Run()
    {
        var announcement = Announcement.Create("Feriado", null, TestData.Now, TestData.Today, visibleUntil: TestData.Today).Value!;

        var result = announcement.Edit("Feriado del lunes", null, AnnouncementCategory.News, null, TestData.Today, TestData.Today.AddDays(5));

        result.IsSuccess.ShouldBeTrue();
        announcement.VisibleUntil.ShouldBe(TestData.Today);
    }
}
