using ClassManager.Core.UseCases.PrivateLessons;

namespace ClassManager.Core.U.Tests.UseCases.PrivateLessons.When_ListInstructorAgenda_has_a_cancelled_lesson;

public sealed class Then_the_lesson_is_not_listed
{
    [Fact]
    public async Task Then_the_lesson_is_not_listed_Run()
    {
        var builder = new PrivateLessonUseCaseBuilder();
        var lesson = builder.ExistingLesson(TestData.Today, "10:00");
        lesson.Cancel(null);
        builder.InstructorHasLessons(lesson);

        var response = await builder.BuildListAgenda().ExecuteAsync(
            new ListInstructorAgendaQuery(builder.Instructor.Id, TestData.Today), CancellationToken.None);

        response.Value!.ShouldBeEmpty();
    }
}
