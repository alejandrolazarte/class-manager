using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Fees;

public sealed class DefaultMonthlyFeeChange : ITenantOwned
{
    private DefaultMonthlyFeeChange()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public decimal? Amount { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static Result<DefaultMonthlyFeeChange> Create(BillingMonth effectiveFrom, decimal? amount, DateOnly today, DateTimeOffset createdAt)
    {
        if (!effectiveFrom.IsWithinAllowedRange(today))
        {
            return Result.Validation<DefaultMonthlyFeeChange>(BillingMonth.AllowedRangeMessage, fieldName: nameof(EffectiveFrom));
        }

        var amountValidation = MonthlyFee.Validate(amount, nameof(Amount));
        if (amountValidation.IsFailure)
        {
            return amountValidation.Error!;
        }

        return new DefaultMonthlyFeeChange
        {
            Id = Guid.CreateVersion7(),
            EffectiveFrom = effectiveFrom.FirstDay,
            Amount = amount,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public void ReplaceWith(DefaultMonthlyFeeChange change) => Amount = change.Amount;
}
