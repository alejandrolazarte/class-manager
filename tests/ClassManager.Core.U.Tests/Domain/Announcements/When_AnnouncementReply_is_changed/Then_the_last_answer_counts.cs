using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.U.Tests.Domain.Announcements.When_AnnouncementReply_is_changed;

public sealed class Then_the_last_answer_counts
{
    [Fact]
    public void Then_the_last_answer_counts_Run()
    {
        var announcement = Announcement.Create(
            "Muestra de fin de año", null, TestData.Now, TestData.Today, AnnouncementCategory.Event).Value!.WithId();
        var reply = AnnouncementReply.Create(announcement, TestIds.Next(), isGoing: true, TestData.Now).Value!;

        reply.Change(isGoing: false, TestData.Now.AddHours(1));

        reply.IsGoing.ShouldBeFalse();
        reply.RepliedAt.ShouldBe(TestData.Now.AddHours(1));
    }
}
