using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.U.Tests.UseCases.PrivateLessons.When_SchedulePrivateLesson_for_an_instructor_who_substitutes;

public sealed class Then_returns_instructor_busy
{
    [Fact]
    public async Task Then_returns_instructor_busy_Run()
    {
        var builder = new PrivateLessonUseCaseBuilder();
        var classGroup = builder.InstructorTeachesGroupAt(builder.Instructor.Id, "18:30");
        builder.SubstituteTodayIn(classGroup, builder.OtherInstructor.Id);

        var response = await builder.BuildSchedule().ExecuteAsync(builder.ScheduleCommand(builder.OtherInstructor.Id), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClassGroupErrorCodes.InstructorBusy);
        builder.AddedLessons.ShouldBeEmpty();
    }
}
