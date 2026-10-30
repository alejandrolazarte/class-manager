using ClassManager.Records;
using ClassManager.Subscriptions.AspNetCore.Persistence;
using ClassManager.Subscriptions.Billing;
using ClassManager.Subscriptions.Subscribers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClassManager.Subscriptions.AspNetCore.Billing;

internal sealed partial class BillingWebhookProcessor<TDbContext>(
    TDbContext context,
    IBillingGateway gateway,
    TimeProvider timeProvider,
    ILogger<BillingWebhookProcessor<TDbContext>> logger) : IBillingWebhookProcessor
    where TDbContext : DbContext, ISubscriptionsDbContext
{
    public async Task ProcessAsync(BillingEvent billingEvent, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(billingEvent);

        if (await context.ProcessedBillingEvents.AnyAsync(processed => processed.EventId == billingEvent.Id, cancellationToken))
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        if (billingEvent.ExternalSubscriptionId is { } externalSubscriptionId)
        {
            await SyncAsync(billingEvent.Id, externalSubscriptionId, now, cancellationToken);
        }

        context.ProcessedBillingEvents.Add(ProcessedBillingEvent.Create(billingEvent.Id, now));
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task SyncAsync(string eventId, string externalSubscriptionId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var external = await gateway.GetSubscriptionAsync(externalSubscriptionId, cancellationToken);
        if (external is null)
        {
            LogSubscriptionNotFound(eventId, externalSubscriptionId);
            return;
        }

        var current = external.SubscriberId is { } subscriberId
            ? await context.Subscriptions.WhereNotDeleted().SingleOrDefaultAsync(subscription => subscription.SubscriberId == subscriberId, cancellationToken)
            : null;
        var planCodes = (await context.Plans.Select(plan => plan.Code).ToListAsync(cancellationToken)).ToHashSet();

        var result = ExternalSubscriptionSync.Apply(current, external, planCodes, now);
        if (result.Started is { } started)
        {
            context.Subscriptions.Add(started);
            await MarkOfferPaidAsync(started, now, cancellationToken);
        }

        if (result.Outcome is ExternalSubscriptionSyncOutcome.MissingSubscriber or ExternalSubscriptionSyncOutcome.UnknownPlan)
        {
            LogNotApplied(eventId, externalSubscriptionId, result.Outcome);
        }
    }

    private async Task MarkOfferPaidAsync(Subscription started, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var offer = await context.PlanOffers.WhereNotDeleted()
            .SingleOrDefaultAsync(candidate => candidate.SubscriberId == started.SubscriberId, cancellationToken);
        if (offer is not null && offer.IsPaidBy(started))
        {
            offer.Delete(now);
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Billing event {EventId}: external subscription {ExternalSubscriptionId} was not found")]
    private partial void LogSubscriptionNotFound(string eventId, string externalSubscriptionId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Billing event {EventId}: external subscription {ExternalSubscriptionId} was not applied ({Outcome})")]
    private partial void LogNotApplied(string eventId, string externalSubscriptionId, ExternalSubscriptionSyncOutcome outcome);
}
