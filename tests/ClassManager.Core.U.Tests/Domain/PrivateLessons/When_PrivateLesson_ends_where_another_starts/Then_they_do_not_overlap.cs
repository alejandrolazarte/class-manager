using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.PrivateLessons;

namespace ClassManager.Core.U.Tests.Domain.PrivateLessons.When_PrivateLesson_ends_where_another_starts;

public sealed class Then_they_do_not_overlap
{
    [Fact]
    public void Then_they_do_not_overlap_Run()
    {
        var schedule = ClassSchedule.Create([TestData.Today.DayOfWeek], "18:00", 45).Value!;
        var lesson = PrivateLesson.Create(Guid.CreateVersion7(), TestData.Today, schedule, [Guid.CreateVersion7()], null, null, null, TestData.Now).Value!;

        var overlaps = (
            BackToBack: lesson.OverlapsWith(TestData.Today, new TimeOnly(18, 45), 45),
            Overlapping: lesson.OverlapsWith(TestData.Today, new TimeOnly(18, 30), 45));

        overlaps.ShouldBe((false, true));
    }
}
