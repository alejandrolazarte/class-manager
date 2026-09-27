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
        var lesson = PrivateLesson.Create(Guid.CreateVersion7(), TestData.Today, schedule, [Guid.CreateVersion7()], null, null, null, TestData.Now).Value!;

        var mark = lesson.Mark(Guid.CreateVersion7(), AttendanceStatus.Present);

        mark.Error!.Code.ShouldBe(PrivateLessonErrorCodes.StudentNotInLesson);
    }
}
