using ClassManager.Subscriptions.Offers;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Core.Abstractions.Subscriptions;

public interface ISelfServiceBillingService
{
    Task<SelfServiceBillingAccess> GetAccessAsync(Subscription subscription, CancellationToken cancellationToken);

    Task<PlanOffer?> FindActiveOfferAsync(long subscriberId, CancellationToken cancellationToken);

    Task<SelfServiceBillingAccess> GetAccessAsync(PlanOffer offer, CancellationToken cancellationToken);
}
