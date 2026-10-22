using ClassManager.Core.Common;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Fees;

public sealed class DefaultMonthlyFeeChange : ITenantOwned, IAuditable, ISoftDeletable
{
    private DefaultMonthlyFeeChange()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public BusinessBillingMode BillingMode { get; private set; }
    public decimal? Amount { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }
    public bool IsDeleted { get; private set; }

    public static Result<DefaultMonthlyFeeChange> Create(
        BillingMonth effectiveFrom,
        BusinessBillingMode billingMode,
        decimal? amount,
        DateOnly today,
        DateTimeOffset createdAt)
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
            EffectiveFrom = effectiveFrom.FirstDay,
            BillingMode = billingMode,
            Amount = amount,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public void ReplaceWith(DefaultMonthlyFeeChange change)
    {
        BillingMode = change.BillingMode;
        Amount = change.Amount;
    }

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }
}
