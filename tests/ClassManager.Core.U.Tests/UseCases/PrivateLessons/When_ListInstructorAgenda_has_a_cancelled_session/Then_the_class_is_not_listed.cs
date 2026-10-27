using ClassManager.Core.UseCases.PrivateLessons;

namespace ClassManager.Core.U.Tests.UseCases.PrivateLessons.When_ListInstructorAgenda_has_a_cancelled_session;

public sealed class Then_the_class_is_not_listed
{
    [Fact]
    public async Task Then_the_class_is_not_listed_Run()
    {
        var builder = new PrivateLessonUseCaseBuilder();
        var classGroup = builder.InstructorTeachesGroupAt(builder.Instructor.Id, "18:30");
        builder.CancelTodayIn(classGroup);

        var response = await builder.BuildListAgenda().ExecuteAsync(
            new ListInstructorAgendaQuery(builder.Instructor.Id, TestData.Today), CancellationToken.None);

        response.Value!.ShouldBeEmpty();
    }
}
