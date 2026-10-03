using ClassManager.Infrastructure.Subscriptions;
using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;
using Microsoft.Extensions.Configuration;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionRepository(AppDbContext context, IConfiguration configuration) : ISubscriptionRepository
{
    public Task<Plan> GetSignUpPlanAsync(CancellationToken cancellationToken) =>
        configuration[SubscriptionSettings.SignUpPlan] is { Length: > 0 } signUpPlanCode
            ? context.Plans.AsNoTracking().SingleAsync(plan => plan.Code == signUpPlanCode, cancellationToken)
            : context.Plans.AsNoTracking().SingleAsync(plan => plan.IsDefault, cancellationToken);

    public void Add(Subscription subscription) => context.Subscriptions.Add(subscription);
}
