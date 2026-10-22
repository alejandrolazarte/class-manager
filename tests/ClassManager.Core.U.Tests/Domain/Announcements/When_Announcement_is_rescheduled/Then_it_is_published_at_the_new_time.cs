using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_is_rescheduled;

public sealed class Then_it_is_published_at_the_new_time
{
    [Fact]
    public void Then_it_is_published_at_the_new_time_Run()
    {
        var announcement = Announcement.Create("Feriado", null, TestData.Now.AddDays(2), TestData.Today).Value!.WithId();

        announcement.Reschedule(TestData.Now.AddDays(4));

        announcement.IsPublishedBy(TestData.Now.AddDays(3)).ShouldBeFalse();
        announcement.IsPublishedBy(TestData.Now.AddDays(4)).ShouldBeTrue();
    }
}
