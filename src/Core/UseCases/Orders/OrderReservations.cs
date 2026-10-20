using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.UseCases.Orders;

internal static class OrderReservations
{
    public static async Task ReleaseAsync(
        Order order,
        IStockMovementRepository stockMovementRepository,
        Guid? recordedByUserId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        foreach (var reservation in await stockMovementRepository.ListReservationsByOrderAsync(order.Id, cancellationToken))
        {
            stockMovementRepository.Add(StockMovement.Release(reservation, recordedByUserId, now));
        }
    }
}
