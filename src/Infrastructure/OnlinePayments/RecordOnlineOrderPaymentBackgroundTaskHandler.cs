using ClassManager.Core.UseCases;
using ClassManager.Core.UseCases.OnlinePayments;
using ClassManager.Infrastructure.BackgroundTasks;
using Microsoft.Extensions.Logging;

namespace ClassManager.Infrastructure.OnlinePayments;

internal sealed partial class RecordOnlineOrderPaymentBackgroundTaskHandler(
    IUseCase<RecordOnlineOrderPaymentCommand, bool> recordPayment,
    ILogger<RecordOnlineOrderPaymentBackgroundTaskHandler> logger)
    : IBackgroundTaskHandler<RecordOnlineOrderPaymentBackgroundTaskCommand>
{
    public async Task HandleAsync(RecordOnlineOrderPaymentBackgroundTaskCommand command, CancellationToken cancellationToken)
    {
        var result = await recordPayment.ExecuteAsync(
            new RecordOnlineOrderPaymentCommand(command.OrderId, command.Amount, command.ExternalPaymentId),
            cancellationToken);
        if (result.IsFailure)
        {
            LogNotRecorded(logger, command.ExternalPaymentId, command.BusinessId, command.OrderId, result.Error!.Code);
        }
    }

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Online payment {ExternalPaymentId} of branch {BusinessId} was not recorded on order {OrderId} ({Code}); refund it from the branch's Stripe dashboard")]
    private static partial void LogNotRecorded(ILogger logger, string externalPaymentId, long businessId, long orderId, string code);
}
