using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.PrivateLessons;

namespace ClassManager.Core.U.Tests.Domain.PrivateLessons.When_PrivateLesson_has_five_students;

public sealed class Then_returns_validation
{
    [Fact]
    public void Then_returns_validation_Run()
    {
        var schedule = ClassSchedule.Create([TestData.Today.DayOfWeek], "18:00", 45).Value!;
        long[] fiveStudents = [.. Enumerable.Range(0, 5).Select(_ => TestIds.Next())];

        var lesson = PrivateLesson.Create(TestIds.Next(), TestData.Today, schedule, fiveStudents, null, null, null, TestData.Now);

        lesson.Error!.FieldName.ShouldBe("StudentIds");
    }
}
