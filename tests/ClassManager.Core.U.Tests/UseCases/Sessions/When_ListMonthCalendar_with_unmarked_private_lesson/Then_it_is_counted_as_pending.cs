using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.PrivateLessons;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_ListMonthCalendar_with_unmarked_private_lesson;

public sealed class Then_it_is_counted_as_pending
{
    [Fact]
    public async Task Then_it_is_counted_as_pending_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var lessonDate = TestData.Today.AddDays(-2);
        var lesson = PrivateLesson.Create(
            builder.Instructor.Id,
            lessonDate,
            ClassSchedule.Create([lessonDate.DayOfWeek], "10:00", 45).Value!,
            [builder.EnrolledStudentId],
            null,
            null,
            null,
            TestData.Now).Value!;
        builder.SetupMonthCalendar([], [], new Dictionary<Guid, AttendanceCount>());
        builder.PrivateLessons
            .Setup(repository => repository.ListBetweenAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([lesson]);

        var response = await builder.BuildMonthCalendar().ExecuteAsync(new ListMonthCalendarQuery("2026-09"), CancellationToken.None);

        var lessonDay = response.Value!.Days.Single(day => day.Date == lessonDate);
        (lessonDay.ClassCount, lessonDay.PendingAttendanceCount).ShouldBe((1, 1));
    }
}
