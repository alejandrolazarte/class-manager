using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_ListMonthCalendar_with_cancelled_class;

public sealed class Then_it_is_counted_as_cancelled_and_not_pending
{
    [Fact]
    public async Task Then_it_is_counted_as_cancelled_and_not_pending_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var cancelledDate = TestData.Today.AddDays(-7);
        var cancelledSession = ClassSession.Create(builder.ClassGroup.Id, cancelledDate, TestData.Now);
        cancelledSession.Cancel("Feriado");
        builder.SetupMonthCalendar(
            [new ClassGroupEnrollmentPeriod(builder.ClassGroup.Id, TestData.Today.AddDays(-30), null)],
            [cancelledSession],
            new Dictionary<Guid, AttendanceCount>());

        var response = await builder.BuildMonthCalendar().ExecuteAsync(new ListMonthCalendarQuery("2026-09"), CancellationToken.None);

        var cancelledDay = response.Value!.Days.Single(day => day.Date == cancelledDate);
        cancelledDay.CancelledCount.ShouldBe(1);
        cancelledDay.PendingAttendanceCount.ShouldBe(0);
    }
}
