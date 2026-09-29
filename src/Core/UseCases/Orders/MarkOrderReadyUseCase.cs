using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Orders;

public sealed record MarkOrderReadyCommand(Guid OrderId);

public sealed class MarkOrderReadyUseCase(
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IClassGroupRepository classGroupRepository,
    IUnitOfWork unitOfWork,
    IAccessScopes accessScopes,
    IOrderNotificationService orderNotifications,
    TimeProvider timeProvider)
    : IUseCase<MarkOrderReadyCommand, OrderResponse>
{
    public async Task<Result<OrderResponse>> ExecuteAsync(MarkOrderReadyCommand command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetForUpdateAsync(command.OrderId, cancellationToken);
        if (order is null || !await OrderAccess.CanReachAsync(accessScopes, clientRepository, order, cancellationToken))
        {
            return OrderFailures.NotFound();
        }

        var ready = order.MarkReady(timeProvider.GetUtcNow());
        if (ready.IsFailure)
        {
            return ready.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await orderNotifications.OrderReadyAsync(order, cancellationToken);

        return await OrderResponses.OfAsync(order, clientRepository, classGroupRepository, cancellationToken);
    }
}
