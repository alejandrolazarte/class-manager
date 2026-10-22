using ClassManager.Backoffice.Core.Brands;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases;
using ClassManager.Records;
using ClassManager.Subscriptions.Access;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Core.UseCases;

[AuditedQuery]
public sealed record GetBrandQuery(long BrandId) : IQuery;

public sealed record BrandOwnerResponse(string FullName, string? Email);

public sealed record BrandCurrentSubscriptionResponse(
    string PlanCode,
    decimal Price,
    string Currency,
    string? Note,
    DateTimeOffset CreatedOn,
    DateOnly? ExpiredOn,
    BrandStatus Status,
    int? TrialDaysLeft);

public sealed record BrandFeatureResponse(string Code, int? Limit);

public sealed record BrandBranchResponse(
    long Id,
    string Name,
    string CountryCode,
    DateTimeOffset CreatedAt,
    int StudentCount,
    int TeamCount,
    int StorageMegabytes);

public sealed record BrandAddOnResponse(
    long Id,
    string FeatureCode,
    decimal Price,
    string Currency,
    int? Limit,
    DateTimeOffset CreatedOn,
    DateOnly? ExpiredOn)
{
    public static BrandAddOnResponse From(SubscriptionFeature addOn) =>
        new(addOn.Id, addOn.FeatureCode, addOn.Price, addOn.Currency, addOn.Limit, addOn.CreatedOn, addOn.ExpiredOn);
}

public sealed record BrandPastSubscriptionResponse(
    string PlanCode,
    decimal Price,
    string Currency,
    string? Note,
    DateTimeOffset CreatedOn,
    DateOnly? ExpiredOn,
    DateTimeOffset? DeletedAt);

public sealed record BrandDetailResponse(
    long Id,
    string Name,
    DateTimeOffset CreatedAt,
    IReadOnlyList<BrandOwnerResponse> Owners,
    BrandStatus Status,
    BrandCurrentSubscriptionResponse? Subscription,
    IReadOnlyList<BrandFeatureResponse> Features,
    IReadOnlyList<BrandBranchResponse> Branches,
    IReadOnlyList<BrandAddOnResponse> AddOns,
    IReadOnlyList<BrandPastSubscriptionResponse> PastSubscriptions);

public sealed class GetBrandUseCase(IBrandDirectory brandDirectory, TimeProvider timeProvider)
    : IUseCase<GetBrandQuery, BrandDetailResponse>
{
    private const long BytesPerMegabyte = 1024 * 1024;

    public async Task<Result<BrandDetailResponse>> ExecuteAsync(GetBrandQuery command, CancellationToken cancellationToken)
    {
        if (await brandDirectory.FindAsync(command.BrandId, cancellationToken) is not { } brand)
        {
            return Result.NotFound<BrandDetailResponse>(BrandErrorCodes.BrandNotFoundMessage, BrandErrorCodes.BrandNotFound);
        }

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var current = brand.CurrentSubscription;
        var activeAddOns = brand.CurrentSubscriptionFeatures.Where(addOn => addOn.IsActiveOn(today)).ToList();
        var features = current is null
            ? []
            : EffectiveFeatures.Combine(current.PlanCode, brand.CurrentPlanFeatures, activeAddOns, today).All
                .OrderBy(feature => feature.Code, StringComparer.Ordinal)
                .Select(feature => new BrandFeatureResponse(feature.Code, feature.Limit))
                .ToList();

        return new BrandDetailResponse(
            brand.Id,
            brand.Name,
            brand.CreatedAt,
            [.. brand.Owners.Select(owner => new BrandOwnerResponse(owner.FullName, owner.Email))],
            BrandSubscriptionStatus.Of(current, today),
            current is null ? null : new BrandCurrentSubscriptionResponse(
                current.PlanCode,
                current.Price,
                current.Currency,
                current.Note,
                current.CreatedOn,
                current.ExpiredOn,
                BrandSubscriptionStatus.Of(current, today),
                BrandSubscriptionStatus.TrialDaysLeft(current, today)),
            features,
            [.. brand.Branches
                .OrderBy(branch => branch.CreatedAt)
                .Select(branch => new BrandBranchResponse(
                    branch.Id,
                    branch.Name,
                    branch.CountryCode,
                    branch.CreatedAt,
                    branch.StudentCount,
                    branch.TeamCount,
                    (int)((branch.DocumentBytes + BytesPerMegabyte - 1) / BytesPerMegabyte)))],
            [.. activeAddOns
                .OrderBy(addOn => addOn.CreatedOn)
                .Select(addOn => BrandAddOnResponse.From(addOn))],
            [.. brand.Subscriptions
                .Where(subscription => subscription.IsDeleted)
                .OrderByDescending(subscription => subscription.CreatedOn)
                .Select(subscription => new BrandPastSubscriptionResponse(
                    subscription.PlanCode,
                    subscription.Price,
                    subscription.Currency,
                    subscription.Note,
                    subscription.CreatedOn,
                    subscription.ExpiredOn,
                    subscription.DeletedAt))]);
    }
}
