using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class OrderRepository(AppDbContext context) : IOrderRepository
{
    public void Add(Order order) => context.Orders.Add(order);

    public Task<Order?> GetByIdAsync(long orderId, CancellationToken cancellationToken) =>
        context.Orders.AsNoTracking().Include(order => order.Lines).Include(order => order.Payments).AsSplitQuery()
            .FirstOrDefaultAsync(order => order.Id == orderId, cancellationToken);

    public Task<Order?> GetForUpdateAsync(long orderId, CancellationToken cancellationToken) =>
        context.Orders.Include(order => order.Lines).Include(order => order.Payments).AsSplitQuery()
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

        if (criteria.CreatorUserId is { } creatorUserId)
        {
            orders = orders.Where(order => order.CreatedByUserId == creatorUserId);
        }

        if (criteria.HiddenCreatorUserIds is { Count: > 0 } hiddenCreatorUserIds)
        {
            orders = orders.Where(order => order.CreatedByUserId == null || !hiddenCreatorUserIds.Contains(order.CreatedByUserId.Value));
        }

        if (criteria.OwingOnly)
        {
            orders = orders.Where(OrderQueries.IsOwing());
        }

        if (criteria.AwaitingPickupOnly)
        {
            orders = orders.Where(order =>
                order.Status == OrderStatus.Pending && order.Lines.Any(line => line.Kind == OrderLineKind.Product));
        }

        return await orders
            .Include(order => order.Lines)
            .Include(order => order.Payments)
            .OrderByDescending(order => order.CreatedAt)
            .Take(criteria.Limit)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> ListPaidBetweenOrOwingAsync(
        DateOnly firstDay, DateOnly lastDay, ClientScope? scope, CancellationToken cancellationToken)
    {
        var orders = context.Orders.AsNoTracking();
        if (scope is not null)
        {
            var clientIdsInScope = ClientScopeQuery.ClientIdsIn(context, scope);
            orders = orders.Where(order => order.ClientId != null && clientIdsInScope.Contains(order.ClientId.Value));
        }

        return await orders
            .Where(order => order.Status != OrderStatus.Cancelled
                && (order.Lines.Sum(line => line.UnitPrice * line.Quantity - line.RefundedAmount) > order.Payments.Sum(payment => payment.Amount)
                    || order.Payments.Any(payment => payment.PaidOn >= firstDay && payment.PaidOn <= lastDay)))
            .Include(order => order.Lines)
            .Include(order => order.Payments)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountUnpaidRequestsByClientAsync(long clientId, CancellationToken cancellationToken) =>
        context.Orders.Where(order => order.ClientId == clientId).CountAsync(OrderQueries.IsUnpaidRequest(), cancellationToken);

    public async Task<IReadOnlyList<Order>> ListUnpaidRequestsCreatedBeforeAsync(DateTimeOffset createdBefore, CancellationToken cancellationToken) =>
        await context.Orders
            .Include(order => order.Lines)
            .Include(order => order.Payments)
            .Where(OrderQueries.IsUnpaidRequestCreatedBefore(createdBefore))
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Order>> ListToDeliverInClassAsync(long classGroupId, CancellationToken cancellationToken) =>
        await context.Orders.AsNoTracking()
            .Include(order => order.Lines)
            .Include(order => order.Payments)
            .Where(order => order.Status == OrderStatus.Pending
                && order.Lines.Any(line => line.Kind == OrderLineKind.Product)
                && order.Delivery == DeliveryMethod.InClass
                && order.DeliveryClassGroupId == classGroupId)
            .OrderBy(order => order.CreatedAt)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

    public Task<Order?> GetForUpdateByPurchaseAsync(long purchaseId, CancellationToken cancellationToken) =>
        context.Orders.Include(order => order.Lines).Include(order => order.Payments).AsSplitQuery()
            .FirstOrDefaultAsync(order => order.Lines.Any(line => line.ClassPackPurchaseId == purchaseId), cancellationToken);

    public void RemovePayment(OrderPayment payment) => context.OrderPayments.Remove(payment);

    public Task<OrderPayment?> FindDeletedPaymentForUpdateAsync(long orderId, long paymentId, CancellationToken cancellationToken) =>
        context.OrderPayments
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .FirstOrDefaultAsync(payment => payment.Id == paymentId && payment.OrderId == orderId && payment.IsDeleted, cancellationToken);
}
