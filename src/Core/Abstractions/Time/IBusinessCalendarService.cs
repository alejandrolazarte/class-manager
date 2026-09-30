namespace ClassManager.Core.Abstractions.Time;

public interface IBusinessCalendarService
{
    Task<DateOnly> TodayAsync(CancellationToken cancellationToken);

    Task<DateTime> LocalNowAsync(CancellationToken cancellationToken);
}
