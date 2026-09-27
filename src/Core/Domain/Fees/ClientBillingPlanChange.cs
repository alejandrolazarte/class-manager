using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Fees;

public sealed class ClientBillingPlanChange : ITenantOwned
{
    private const string KindRequiredMessage = "Choose how the family pays.";
    private const string CustomFeeRequiredMessage = "Enter the family's monthly fee.";

    private ClientBillingPlanChange()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClientId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public BillingPlanKind Kind { get; private set; }
    public decimal? CustomFee { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public BillingPlan Plan => new(Kind, CustomFee);

    public static Result<ClientBillingPlanChange> Create(
        Guid clientId,
        BillingMonth effectiveFrom,
        BillingPlanKind? kind,
        decimal? customFee,
        DateOnly today,
        DateTimeOffset createdAt)
    {
        if (!effectiveFrom.IsWithinAllowedRange(today))
        {
            return Result.Validation<ClientBillingPlanChange>(BillingMonth.AllowedRangeMessage, fieldName: nameof(EffectiveFrom));
        }

        if (kind is null)
        {
            return Result.Validation<ClientBillingPlanChange>(KindRequiredMessage, fieldName: nameof(Kind));
        }

        if (kind == BillingPlanKind.CustomFee && customFee is null)
        {
            return Result.Validation<ClientBillingPlanChange>(CustomFeeRequiredMessage, fieldName: nameof(CustomFee));
        }

        var feeValidation = MonthlyFee.Validate(customFee, nameof(CustomFee));
        if (feeValidation.IsFailure)
        {
            return feeValidation.Error!;
        }

        return new ClientBillingPlanChange
        {
            Id = Guid.CreateVersion7(),
            ClientId = clientId,
            EffectiveFrom = effectiveFrom.FirstDay,
            Kind = kind.Value,
            CustomFee = kind == BillingPlanKind.CustomFee ? customFee : null,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public void ReplaceWith(ClientBillingPlanChange change)
    {
        Kind = change.Kind;
        CustomFee = change.CustomFee;
    }
}
