using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Subscriptions;

public sealed record ListPlansQuery : IQuery;

public sealed class ListPlansUseCase(ISubscriptionRepository subscriptionRepository)
    : IUseCase<ListPlansQuery, IReadOnlyList<PlanResponse>>
{
    public async Task<Result<IReadOnlyList<PlanResponse>>> ExecuteAsync(ListPlansQuery command, CancellationToken cancellationToken)
    {
        var plans = await subscriptionRepository.ListActivePlansAsync(cancellationToken);
        var featuresByPlanCode = (await subscriptionRepository.ListPlanFeaturesAsync(cancellationToken))
            .ToLookup(planFeature => planFeature.PlanCode, StringComparer.Ordinal);

        return plans
            .Select(plan => new PlanResponse(
                plan.Code,
                plan.ListPrice,
                plan.Currency,
                plan.BillingPeriod,
                plan.DurationInDays,
                [.. featuresByPlanCode[plan.Code]
                    .OrderBy(planFeature => planFeature.FeatureCode, StringComparer.Ordinal)
                    .Select(planFeature => new FeatureLimitResponse(planFeature.FeatureCode, planFeature.Limit))]))
            .ToList();
    }
}
