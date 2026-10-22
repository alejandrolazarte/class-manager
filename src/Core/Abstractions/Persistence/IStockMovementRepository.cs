using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IStockMovementRepository
{
    void Add(StockMovement movement);

    Task<IReadOnlyDictionary<long, int>> StockByVariantAsync(IReadOnlyCollection<long> variantIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<StockMovement>> ListReservationsByOrderAsync(long orderId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StockMovement>> ListByProductAsync(long productId, int limit, CancellationToken cancellationToken);
}
