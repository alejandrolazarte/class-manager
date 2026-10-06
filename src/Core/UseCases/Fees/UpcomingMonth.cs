using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

internal static class UpcomingMonth
{
    private const string NotUpcomingMessage = "Only fee changes for upcoming months can be deleted.";

    public static Result<BillingMonth> Resolve(string? month, DateOnly today, string fieldName)
    {
        var parsedMonth = BillingMonth.Parse(month, fieldName);
        if (parsedMonth.IsFailure)
        {
            return parsedMonth;
        }

        return parsedMonth.Value!.FirstDay > BillingMonth.From(today).FirstDay
            ? parsedMonth
            : Result.Validation<BillingMonth>(NotUpcomingMessage, fieldName: fieldName);
    }
}
