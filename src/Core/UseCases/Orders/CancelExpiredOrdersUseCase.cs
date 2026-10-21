using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.Orders;

public sealed record CancelExpiredOrdersCommand : ICommand;

public sealed class CancelExpiredOrdersUseCase(
    IOrderRepository orderRepository,
    IStockMovementRepository stockMovementRepository,
    IUnitOfWork unitOfWork,
    IOrderNotificationService orderNotifications,
    TimeProvider timeProvider)
    : IUseCase<CancelExpiredOrdersCommand, int>
{
    public async Task<Result<int>> ExecuteAsync(CancelExpiredOrdersCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var expiredOrders = await orderRepository.ListUnpaidRequestsCreatedBeforeAsync(now - Order.RequestLifetime, cancellationToken);
        foreach (var order in expiredOrders)
        {
            order.CancelUnpaid(null, now);
            await OrderReservations.ReleaseAsync(order, stockMovementRepository, null, now, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        foreach (var order in expiredOrders)
        {
            await orderNotifications.OrderCancelledAsync(order, OrderCancellationReason.Unpaid, cancellationToken);
        }

        return expiredOrders.Count;
    }
}
