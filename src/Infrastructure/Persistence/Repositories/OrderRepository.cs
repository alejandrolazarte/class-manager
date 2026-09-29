using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class OrderRepository(AppDbContext context) : IOrderRepository
{
    public void Add(Order order) => context.Orders.Add(order);

    public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken) =>
        context.Orders.AsNoTracking().Include(order => order.Lines)
            .FirstOrDefaultAsync(order => order.Id == orderId, cancellationToken);

    public Task<Order?> GetForUpdateAsync(Guid orderId, CancellationToken cancellationToken) =>
        context.Orders.Include(order => order.Lines)
            .FirstOrDefaultAsync(order => order.Id == orderId, cancellationToken);

    public async Task<IReadOnlyList<Order>> ListAsync(
        OrderSearchCriteria criteria, ClientScope? scope, CancellationToken cancellationToken)
    {
        var orders = context.Orders.AsNoTracking();
        if (scope is not null)
        {
            var clientIdsInScope = ClientScopeQuery.ClientIdsIn(context, scope);
            orders = orders.Where(order => order.ClientId != null && clientIdsInScope.Contains(order.ClientId.Value));
        }

        if (criteria.ClientId is { } clientId)
        {
            orders = orders.Where(order => order.ClientId == clientId);
        }

        if (criteria.AwaitingPickupOnly)
        {
            orders = orders.Where(order =>
                order.Status == OrderStatus.Paid && order.Lines.Any(line => line.Kind == OrderLineKind.Product));
        }

        return await orders
            .Include(order => order.Lines)
            .OrderByDescending(order => order.CreatedAt)
            .Take(criteria.Limit)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsPurchaseFromOrderAsync(Guid purchaseId, CancellationToken cancellationToken) =>
        context.OrderLines.AsNoTracking().AnyAsync(line => line.ClassPackPurchaseId == purchaseId, cancellationToken);
}
