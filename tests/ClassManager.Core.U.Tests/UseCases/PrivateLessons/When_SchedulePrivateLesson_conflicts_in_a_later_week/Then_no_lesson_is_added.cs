using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.PrivateLessons;

namespace ClassManager.Core.U.Tests.UseCases.PrivateLessons.When_SchedulePrivateLesson_conflicts_in_a_later_week;

public sealed class Then_no_lesson_is_added
{
    [Fact]
    public async Task Then_no_lesson_is_added_Run()
    {
        var builder = new PrivateLessonUseCaseBuilder();
        var busyDate = TestData.Today.AddDays(14);
        builder.PrivateLessons
            .Setup(repository => repository.ListByInstructorOnDatesAsync(builder.Instructor.Id, It.IsAny<IReadOnlyCollection<DateOnly>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([builder.ExistingLesson(busyDate)]);

        var response = await builder.BuildSchedule().ExecuteAsync(builder.ScheduleCommand(builder.Instructor.Id, repeatWeeks: 4), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClassGroupErrorCodes.InstructorBusy);
        response.Error.Details[PrivateLessonErrorCodes.ConflictingDateDetail].ShouldBe(busyDate);
        builder.AddedLessons.ShouldBeEmpty();
    }
}
