using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Core.Abstractions.Subscriptions;

public interface ISelfServiceBillingService
{
    Task<SelfServiceBillingAccess> GetAccessAsync(Subscription subscription, CancellationToken cancellationToken);
}
