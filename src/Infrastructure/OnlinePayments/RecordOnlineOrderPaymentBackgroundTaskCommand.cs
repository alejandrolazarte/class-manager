using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Infrastructure.OnlinePayments;

public sealed record RecordOnlineOrderPaymentBackgroundTaskCommand(long BusinessId, long OrderId, decimal Amount, string ExternalPaymentId)
    : BackgroundTaskCommand(BusinessId);
