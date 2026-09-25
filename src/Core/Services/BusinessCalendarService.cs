using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;

namespace ClassManager.Core.Services;

public sealed class BusinessCalendarService(IBusinessRepository businessRepository, TimeProvider timeProvider) : IBusinessCalendarService
{
    public async Task<DateOnly> TodayAsync(CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var business = await businessRepository.GetCurrentAsync(cancellationToken);

        return business?.TodayAt(now) ?? DateOnly.FromDateTime(now.UtcDateTime);
    }
}
