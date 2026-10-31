using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Fees;

public sealed class ClientBillingPlanChange : ITenantOwned, IAuditable, ISoftDeletable
{
    private const string KindRequiredMessage = "Choose how the client pays.";
    private const string CustomFeeRequiredMessage = "Enter the client's monthly fee.";

    private ClientBillingPlanChange()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public long ClientId { get; private set; }
    public Client? Client { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public BillingPlanKind Kind { get; private set; }
    public decimal? CustomFee { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }
    public bool IsDeleted { get; private set; }

    public BillingPlan Plan => new(Kind, CustomFee);

    public static Result<ClientBillingPlanChange> Create(
        long clientId,
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
            ClientId = clientId,
            EffectiveFrom = effectiveFrom.FirstDay,
            Kind = kind.Value,
            CustomFee = kind == BillingPlanKind.CustomFee ? customFee : null,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public static IReadOnlyList<ClientBillingPlanChange> ForNewClient(
        Client client,
        IReadOnlyCollection<DefaultMonthlyFeeChange> defaultFeeChanges,
        DateOnly today,
        DateTimeOffset createdAt)
    {
        var currentMonth = BillingMonth.From(today);
        var monthsFromNow = defaultFeeChanges
            .Where(change => change.EffectiveFrom > currentMonth.FirstDay)
            .Select(change => BillingMonth.From(change.EffectiveFrom))
            .Prepend(currentMonth)
            .Distinct()
            .OrderBy(month => month.FirstDay);

        var planChanges = new List<ClientBillingPlanChange>();
        foreach (var month in monthsFromNow)
        {
            if (FeeTimeline.BillingModeIn(defaultFeeChanges, month) is not { } billingMode)
            {
                continue;
            }

            var kind = BillingPlan.ForBusiness(billingMode).Kind;
            if (kind == planChanges.LastOrDefault()?.Kind)
            {
                continue;
            }

            planChanges.Add(new ClientBillingPlanChange
            {
                ClientId = client.Id,
                Client = client,
                EffectiveFrom = month.FirstDay,
                Kind = kind,
                CreatedAt = createdAt.ToUniversalTime(),
            });
        }

        return planChanges;
    }

    public void ReplaceWith(ClientBillingPlanChange change)
    {
        Kind = change.Kind;
        CustomFee = change.CustomFee;
    }

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }
}
