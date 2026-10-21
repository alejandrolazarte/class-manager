using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.UseCases.Orders;

internal static class OrderReservations
{
    public static async Task<IReadOnlyList<(Guid VariantId, int Quantity)>> SoldUnitsToRestockAsync(
        Order order, IProductRepository productRepository, CancellationToken cancellationToken)
    {
        var unitsToRestock = new List<(Guid VariantId, int Quantity)>();
        if (order.Channel != OrderChannel.Counter)
        {
            return unitsToRestock;
        }

        foreach (var line in order.Lines.Where(line => line.Kind == OrderLineKind.Product && line.Quantity > line.RefundedQuantity))
        {
            var product = await productRepository.GetByIdAsync(line.ProductId!.Value, cancellationToken);
            if (product is { TracksStock: true })
            {
                unitsToRestock.Add((line.ProductVariantId!.Value, line.Quantity - line.RefundedQuantity));
            }
        }

        return unitsToRestock;
    }

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
