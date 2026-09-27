using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

internal static class EffectiveMonth
{
    public static Result<BillingMonth> Resolve(string? effectiveFrom, DateOnly today, string fieldName) =>
        effectiveFrom is null ? BillingMonth.From(today) : BillingMonth.Parse(effectiveFrom, fieldName);
}
