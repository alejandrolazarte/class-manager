using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.U.Tests.UseCases.PrivateLessons.When_SchedulePrivateLesson_overlaps_a_group_of_the_same_instructor;

public sealed class Then_returns_instructor_busy
{
    [Fact]
    public async Task Then_returns_instructor_busy_Run()
    {
        var builder = new PrivateLessonUseCaseBuilder();
        builder.InstructorTeachesGroupAt(builder.Instructor.Id, "18:30");

        var response = await builder.BuildSchedule().ExecuteAsync(builder.ScheduleCommand(builder.Instructor.Id), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClassGroupErrorCodes.InstructorBusy);
        builder.AddedLessons.ShouldBeEmpty();
    }
}
