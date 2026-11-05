using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Infrastructure.Subscriptions;

public sealed record ProcessBillingEventBackgroundTaskCommand(string EventId, string? ExternalSubscriptionId) : BackgroundTaskCommand(TenantId: null);
