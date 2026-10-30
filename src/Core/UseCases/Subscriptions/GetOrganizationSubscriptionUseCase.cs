using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Subscriptions;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Records;

namespace ClassManager.Core.UseCases.Subscriptions;

public sealed record GetOrganizationSubscriptionQuery : IQuery;

public sealed class GetOrganizationSubscriptionUseCase(
    IBusinessRepository businessRepository,
    ISubscriptionRepository subscriptionRepository,
    ISelfServiceBillingService selfServiceBillingService,
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

        var selfServiceAccess = await selfServiceBillingService.GetAccessAsync(subscription, cancellationToken);
        var isActive = subscription.IsActiveOn(today);
        var paysOnline = subscription.ExternalSubscriptionId is not null && isActive;
        var offer = paysOnline ? null : await selfServiceBillingService.FindActiveOfferAsync(business.OrganizationId, cancellationToken);
        var payableOffer = offer is not null
            && await selfServiceBillingService.GetAccessAsync(offer, cancellationToken) is SelfServiceBillingAccess.Available
                ? new PlanOfferResponse(offer.PlanCode, offer.Price, offer.Currency, offer.ExpiredOn)
                : null;

        return new OrganizationSubscriptionResponse(
            subscription.PlanCode,
            subscription.Price,
            subscription.Currency,
            subscription.CreatedOn,
            subscription.ExpiredOn,
            isActive,
            selfServiceAccess is SelfServiceBillingAccess.Available,
            subscription.ExternalSubscriptionId is not null,
            subscription.PaymentStatus,
            payableOffer);
    }
}
