using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.Orders;

public sealed record CancelExpiredOrdersCommand;

public sealed class CancelExpiredOrdersUseCase(
    IOrderRepository orderRepository,
    IStockMovementRepository stockMovementRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<CancelExpiredOrdersCommand, int>
{
    public async Task<Result<int>> ExecuteAsync(CancelExpiredOrdersCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var expiredOrders = await orderRepository.ListRequestedCreatedBeforeAsync(now - Order.RequestLifetime, cancellationToken);
        foreach (var order in expiredOrders)
        {
            order.Cancel(null, now);
            await OrderPayments.ReleaseReservationsAsync(order, stockMovementRepository, null, now, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return expiredOrders.Count;
    }
}
