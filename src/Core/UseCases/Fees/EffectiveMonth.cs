using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

internal static class EffectiveMonth
{
    private const string PastMonthMessage = "The month can't be before the current month.";

    public static Result<BillingMonth> Resolve(string? effectiveFrom, DateOnly today, string fieldName)
    {
        var currentMonth = BillingMonth.From(today);
        if (effectiveFrom is null)
        {
            return currentMonth;
        }

        var month = BillingMonth.Parse(effectiveFrom, fieldName);
        if (month.IsFailure)
        {
            return month;
        }

        return month.Value!.FirstDay < currentMonth.FirstDay
            ? Result.Validation<BillingMonth>(PastMonthMessage, fieldName: fieldName)
            : month;
    }
}
