using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Subscriptions;

public sealed record GetOrganizationSubscriptionQuery;

public sealed class GetOrganizationSubscriptionUseCase(
    IBusinessRepository businessRepository,
    ISubscriptionRepository subscriptionRepository,
    TimeProvider timeProvider)
    : IUseCase<GetOrganizationSubscriptionQuery, OrganizationSubscriptionResponse>
{
    public const string SubscriptionNotFoundMessage = "The brand has no subscription.";

    public async Task<Result<OrganizationSubscriptionResponse>> ExecuteAsync(GetOrganizationSubscriptionQuery command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<OrganizationSubscriptionResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var subscription = await subscriptionRepository.GetCurrentAsync(business.OrganizationId, cancellationToken);
        if (subscription is null)
        {
            return Result.NotFound<OrganizationSubscriptionResponse>(SubscriptionNotFoundMessage);
        }

        return new OrganizationSubscriptionResponse(
            subscription.PlanCode,
            subscription.Price,
            subscription.Currency,
            subscription.CreatedOn,
            subscription.ExpiredOn,
            subscription.IsActiveOn(today));
    }
}
