namespace ClassManager.Core.U.Tests.UseCases.PrivateLessons.When_SchedulePrivateLesson_repeats_weekly;

public sealed class Then_one_lesson_per_week_is_added_in_a_series
{
    [Fact]
    public async Task Then_one_lesson_per_week_is_added_in_a_series_Run()
    {
        var builder = new PrivateLessonUseCaseBuilder();

        await builder.BuildSchedule().ExecuteAsync(builder.ScheduleCommand(builder.Instructor.Id, repeatWeeks: 3), CancellationToken.None);

        builder.AddedLessons.Select(lesson => lesson.Date).ShouldBe([TestData.Today, TestData.Today.AddDays(7), TestData.Today.AddDays(14)]);
        builder.AddedLessons.Select(lesson => lesson.SeriesId).Distinct().ShouldHaveSingleItem().ShouldNotBeNull();
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
