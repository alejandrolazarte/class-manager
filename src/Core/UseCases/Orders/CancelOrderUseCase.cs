using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Orders;

public sealed record CancelOrderCommand(Guid OrderId);

public sealed class CancelOrderUseCase(
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IClassGroupRepository classGroupRepository,
    IStockMovementRepository stockMovementRepository,
    IUnitOfWork unitOfWork,
    IAccessScopes accessScopes,
    ICurrentMember currentMember,
    IOrderNotificationService orderNotifications,
    TimeProvider timeProvider)
    : IUseCase<CancelOrderCommand, OrderResponse>
{
    public async Task<Result<OrderResponse>> ExecuteAsync(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetForUpdateAsync(command.OrderId, cancellationToken);
        if (order is null || !await OrderAccess.CanReachAsync(accessScopes, clientRepository, order, cancellationToken))
        {
            return OrderFailures.NotFound();
        }

        var access = await currentMember.GetAccessAsync(cancellationToken);
        var now = timeProvider.GetUtcNow();
        var cancellation = order.Cancel(access?.UserId, now);
        if (cancellation.IsFailure)
        {
            return cancellation.Error!;
        }

        await OrderPayments.ReleaseReservationsAsync(order, stockMovementRepository, access?.UserId, now, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await orderNotifications.OrderCancelledAsync(order, OrderCancellationReason.ByBranch, cancellationToken);

        return await OrderResponses.OfAsync(order, clientRepository, classGroupRepository, cancellationToken);
    }
}
