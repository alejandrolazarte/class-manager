using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_is_scheduled;

public sealed class Then_it_is_published_at_its_time
{
    [Fact]
    public void Then_it_is_published_at_its_time_Run()
    {
        var publishOn = TestData.Now.AddDays(2);
        var announcement = Announcement.Create("Feriado", null, publishOn, TestData.Today).Value!;

        announcement.IsPublishedBy(TestData.Now).ShouldBeFalse();
        announcement.IsPublishedBy(publishOn).ShouldBeTrue();
    }
}
