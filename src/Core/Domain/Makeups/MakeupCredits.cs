namespace ClassManager.Core.Domain.Makeups;

public static class MakeupCredits
{
    public const int ValidityDays = 30;

    public static DateOnly FirstBookingDayToLoad(DateOnly today) => today.AddDays(-ValidityDays);

    public static DateOnly FirstMissedDayToLoad(DateOnly today) => today.AddDays(-2 * ValidityDays);

    public static MakeupBalance Calculate(
        IReadOnlyCollection<MakeupSource> sources,
        IReadOnlyCollection<DateOnly> bookedDates,
        DateOnly today)
    {
        var unused = sources.OrderBy(source => source.ExpiresOn).ToList();
        var uncoveredBookings = 0;
        foreach (var bookedDate in bookedDates.Order())
        {
            var source = unused.FirstOrDefault(candidate => candidate.ExpiresOn >= bookedDate);
            if (source is null)
            {
                uncoveredBookings++;
                continue;
            }

            unused.Remove(source);
        }

        return new MakeupBalance(
            [
                .. unused
                    .Where(source => source.ExpiresOn >= today)
                    .Select(source => new MakeupCredit(source.MissedOn, source.Reason, source.ExpiresOn)),
            ],
            uncoveredBookings);
    }
}
