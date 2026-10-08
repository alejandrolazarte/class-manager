using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_ListMonthCalendar_with_unmarked_past_classes;

public sealed class Then_past_days_without_students_are_not_listed
{
    [Fact]
    public async Task Then_past_days_without_students_are_not_listed_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var enrolledSince = TestData.Today.AddDays(-14);
        builder.SetupMonthCalendar(
            [new ClassGroupEnrollmentPeriod(builder.ClassGroup.Id, enrolledSince, null)],
            [],
            new Dictionary<Guid, AttendanceCount>());

        var response = await builder.BuildMonthCalendar().ExecuteAsync(new ListMonthCalendarQuery("2026-09"), CancellationToken.None);

        response.Value!.Days
            .Select(day => (day.Date, day.ClassCount, day.PendingAttendanceCount))
            .ShouldBe(
            [
                (enrolledSince, 1, 1),
                (TestData.Today.AddDays(-7), 1, 1),
                (TestData.Today, 1, 0),
            ]);
    }
}
