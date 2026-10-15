using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_button_goes_to_the_shop;

public sealed class Then_it_drops_the_link
{
    [Fact]
    public void Then_it_drops_the_link_Run()
    {
        var announcement = Announcement.Create(
            "Llegaron remeras", null, TestData.Now, TestData.Today,
            action: AnnouncementAction.Link, actionUrl: "https://example.com").Value!;

        announcement.Edit("Llegaron remeras", null, AnnouncementCategory.News, null, null, TestData.Today, AnnouncementAction.Shop, "https://example.com");

        announcement.Action.ShouldBe(AnnouncementAction.Shop);
        announcement.ActionUrl.ShouldBeNull();
    }
}
