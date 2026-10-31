using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.ClassPacks;

public sealed class ClassPackPriceChange : ITenantOwned, IAuditable, ISoftDeletable
{
    private ClassPackPriceChange()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public long ClassPackId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public decimal Price { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }
    public bool IsDeleted { get; private set; }

    public static Result<ClassPackPriceChange> Create(
        long classPackId,
        BillingMonth effectiveFrom,
        decimal? price,
        DateOnly today,
        DateTimeOffset createdAt)
    {
        if (!effectiveFrom.IsWithinAllowedRange(today))
        {
            return Result.Validation<ClassPackPriceChange>(BillingMonth.AllowedRangeMessage, fieldName: nameof(EffectiveFrom));
        }

        var priceValidation = MonthlyFee.Validate(price ?? 0, nameof(Price));
        if (priceValidation.IsFailure)
        {
            return priceValidation.Error!;
        }

        return new ClassPackPriceChange
        {
            ClassPackId = classPackId,
            EffectiveFrom = effectiveFrom.FirstDay,
            Price = price ?? 0,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public void ReplaceWith(ClassPackPriceChange change) => Price = change.Price;

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }
}
