namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class StockMovementRepository(AppDbContext context) : IStockMovementRepository
{
    public void Add(StockMovement movement) => context.StockMovements.Add(movement);

    public async Task<IReadOnlyDictionary<long, int>> StockByVariantAsync(
        IReadOnlyCollection<long> variantIds, CancellationToken cancellationToken)
    {
        if (variantIds.Count == 0)
        {
            return new Dictionary<long, int>();
        }

        return await context.StockMovements.AsNoTracking()
            .Where(movement => variantIds.Contains(movement.ProductVariantId))
            .GroupBy(movement => movement.ProductVariantId)
            .Select(group => new { VariantId = group.Key, Stock = group.Sum(movement => movement.Quantity) })
            .ToDictionaryAsync(entry => entry.VariantId, entry => entry.Stock, cancellationToken);
    }

    public async Task<IReadOnlyList<StockMovement>> ListReservationsByOrderAsync(long orderId, CancellationToken cancellationToken) =>
        await context.StockMovements.AsNoTracking()
            .Where(movement => movement.OrderId == orderId && movement.Kind == StockMovementKind.Reservation)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StockMovement>> ListByProductAsync(long productId, int limit, CancellationToken cancellationToken) =>
        await (
            from movement in context.StockMovements.AsNoTracking()
            join variant in context.ProductVariants on movement.ProductVariantId equals variant.Id
            where variant.ProductId == productId
            orderby movement.CreatedAt descending
            select movement)
            .Take(limit)
            .ToListAsync(cancellationToken);
}
