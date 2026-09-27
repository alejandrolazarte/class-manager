using System.Globalization;
using ClassManager.Core.Common;

namespace ClassManager.Core.Domain.Fees;

public sealed record BillingMonth
{
    public const string Format = "yyyy-MM";
    public const int MonthsAheadAllowed = 12;
    public const string AllowedRangeMessage = "The month must be between 2000-01 and 12 months from now.";

    public static readonly DateOnly EarliestFirstDay = new(2000, 1, 1);

    private const string FormatMessage = "The month must use the YYYY-MM format.";

    private BillingMonth(DateOnly firstDay)
    {
        FirstDay = firstDay;
    }

    public DateOnly FirstDay { get; }
    public DateOnly LastDay => FirstDay.AddMonths(1).AddDays(-1);

    public static BillingMonth From(DateOnly date) => new(new DateOnly(date.Year, date.Month, 1));

    public static Result<BillingMonth> Parse(string? month, string fieldName = "Month") =>
        DateOnly.TryParseExact(month?.Trim(), Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var firstDay)
            ? new BillingMonth(firstDay)
            : Result.Validation<BillingMonth>(FormatMessage, fieldName: fieldName);

    public BillingMonth AddMonths(int months) => new(FirstDay.AddMonths(months));

    public bool IsWithinAllowedRange(DateOnly today) =>
        FirstDay >= EarliestFirstDay && FirstDay <= From(today).AddMonths(MonthsAheadAllowed).FirstDay;

    public override string ToString() => FirstDay.ToString(Format, CultureInfo.InvariantCulture);
}
