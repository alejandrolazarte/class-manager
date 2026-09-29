using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IStockMovementRepository
{
    void Add(StockMovement movement);

    Task<IReadOnlyDictionary<Guid, int>> StockByVariantAsync(IReadOnlyCollection<Guid> variantIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<StockMovement>> ListReservationsByOrderAsync(Guid orderId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StockMovement>> ListByProductAsync(Guid productId, int limit, CancellationToken cancellationToken);
}
