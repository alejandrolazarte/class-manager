using ClassManager.Core.UseCases.PrivateLessons;

namespace ClassManager.Core.U.Tests.UseCases.PrivateLessons.When_ListInstructorAgenda_while_moving_a_lesson;

public sealed class Then_the_moved_lesson_is_not_listed
{
    [Fact]
    public async Task Then_the_moved_lesson_is_not_listed_Run()
    {
        var builder = new PrivateLessonUseCaseBuilder();
        var movedLesson = builder.ExistingLesson(TestData.Today, "10:00");
        builder.InstructorHasLessons(movedLesson, builder.ExistingLesson(TestData.Today, "12:00"));

        var response = await builder.BuildListAgenda().ExecuteAsync(
            new ListInstructorAgendaQuery(builder.Instructor.Id, TestData.Today, movedLesson.Id), CancellationToken.None);

        response.Value!.ShouldHaveSingleItem().StartTime.ShouldBe("12:00");
    }
}
