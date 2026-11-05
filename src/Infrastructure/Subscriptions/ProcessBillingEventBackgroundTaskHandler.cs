using ClassManager.Infrastructure.BackgroundTasks;
using ClassManager.Subscriptions.AspNetCore.Billing;
using ClassManager.Subscriptions.Billing;

namespace ClassManager.Infrastructure.Subscriptions;

internal sealed class ProcessBillingEventBackgroundTaskHandler(IBillingEventProcessor processor)
    : IBackgroundTaskHandler<ProcessBillingEventBackgroundTaskCommand>
{
    public Task HandleAsync(ProcessBillingEventBackgroundTaskCommand command, CancellationToken cancellationToken) =>
        processor.ProcessAsync(new BillingEvent(command.EventId, command.ExternalSubscriptionId), cancellationToken);
}
