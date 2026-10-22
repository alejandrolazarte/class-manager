using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_Announcement_is_unpinned;

public sealed class Then_it_is_no_longer_pinned
{
    [Fact]
    public void Then_it_is_no_longer_pinned_Run()
    {
        var announcement = Announcement.Create("Horarios de verano", null, TestData.Now, TestData.Today).Value!.WithId();
        announcement.Pin();

        announcement.Unpin();

        announcement.IsPinned.ShouldBeFalse();
    }
}
