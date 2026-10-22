using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_AnnouncementReply_is_for_an_announcement_that_is_not_an_event;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var announcement = Announcement.Create("Horarios de verano", null, TestData.Now, TestData.Today).Value!.WithId();

        var reply = AnnouncementReply.Create(announcement, TestIds.Next(), isGoing: true, TestData.Now);

        reply.Error!.Code.ShouldBe(AnnouncementReply.NotAnEventCode);
    }
}
