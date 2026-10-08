namespace ClassManager.Core.U.Tests.UseCases.PrivateLessons.When_SchedulePrivateLesson_overlaps_a_group_of_another_instructor;

public sealed class Then_the_lesson_is_scheduled
{
    [Fact]
    public async Task Then_the_lesson_is_scheduled_Run()
    {
        var builder = new PrivateLessonUseCaseBuilder();
        builder.InstructorTeachesGroupAt(builder.OtherInstructor.Id, "18:00");

        var response = await builder.BuildSchedule().ExecuteAsync(builder.ScheduleCommand(builder.Instructor.Id), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        builder.AddedLessons.ShouldHaveSingleItem();
    }
}
