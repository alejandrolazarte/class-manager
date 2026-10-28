using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Core.U.Tests.UseCases.ClassGroups.When_ListInstructorWeek_has_a_weekly_lesson_series;

public sealed class Then_each_weekday_and_time_is_listed_once
{
    [Fact]
    public async Task Then_each_weekday_and_time_is_listed_once_Run()
    {
        var builder = new ClassGroupUseCaseBuilder();
        builder.InstructorHasUpcomingLessons(
            builder.ExistingLesson(TestData.Today, "10:00"),
            builder.ExistingLesson(TestData.Today.AddDays(7), "10:00"));

        var response = await builder.BuildListInstructorWeek().ExecuteAsync(
            new ListInstructorWeekQuery(builder.Instructor.Id), CancellationToken.None);

        response.Value!.ShouldHaveSingleItem().StartTime.ShouldBe("10:00");
    }
}
