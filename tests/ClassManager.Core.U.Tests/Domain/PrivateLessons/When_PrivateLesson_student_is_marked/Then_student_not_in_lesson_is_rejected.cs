using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.PrivateLessons;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.PrivateLessons.When_PrivateLesson_student_is_marked;

public sealed class Then_student_not_in_lesson_is_rejected
{
    [Fact]
    public void Then_student_not_in_lesson_is_rejected_Run()
    {
        var schedule = ClassSchedule.Create([TestData.Today.DayOfWeek], "18:00", 45).Value!;
        var lesson = PrivateLesson.Create(TestIds.Next(), TestData.Today, schedule, [TestIds.Next()], null, null, null, TestData.Now).Value!.WithId();

        var mark = lesson.Mark(TestIds.Next(), AttendanceStatus.Present);

        mark.Error!.Code.ShouldBe(PrivateLessonErrorCodes.StudentNotInLesson);
    }
}
