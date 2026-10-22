using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_has_no_category;

public sealed class Then_it_is_news_for_everyone
{
    [Fact]
    public void Then_it_is_news_for_everyone_Run()
    {
        var announcement = Announcement.Create("Horarios de verano", null, TestData.Now, TestData.Today).Value!.WithId();

        announcement.Category.ShouldBe(AnnouncementCategory.News);
        announcement.IsForEveryone.ShouldBeTrue();
        announcement.IsFor([TestIds.Next()]).ShouldBeTrue();
        announcement.IsPinned.ShouldBeFalse();
        announcement.VisibleUntil.ShouldBeNull();
    }
}
