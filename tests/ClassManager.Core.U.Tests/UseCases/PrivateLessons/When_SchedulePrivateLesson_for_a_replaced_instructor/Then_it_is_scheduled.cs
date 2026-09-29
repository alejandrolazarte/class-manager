namespace ClassManager.Core.U.Tests.UseCases.PrivateLessons.When_SchedulePrivateLesson_for_a_replaced_instructor;

public sealed class Then_it_is_scheduled
{
    [Fact]
    public async Task Then_it_is_scheduled_Run()
    {
        var builder = new PrivateLessonUseCaseBuilder();
        var classGroup = builder.InstructorTeachesGroupAt(builder.Instructor.Id, "18:00");
        builder.SubstituteTodayIn(classGroup, builder.OtherInstructor.Id);

        var response = await builder.BuildSchedule().ExecuteAsync(builder.ScheduleCommand(builder.Instructor.Id), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        builder.AddedLessons.ShouldHaveSingleItem();
    }
}
