using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IOrderRepository
{
    void Add(Order order);

    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);

    Task<Order?> GetForUpdateAsync(Guid orderId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> ListAsync(OrderSearchCriteria criteria, ClientScope? scope, CancellationToken cancellationToken);

    Task<int> CountRequestedByClientAsync(Guid clientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> ListRequestedCreatedBeforeAsync(DateTimeOffset createdBefore, CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> ListToDeliverInClassAsync(Guid classGroupId, CancellationToken cancellationToken);

    Task<bool> IsPurchaseFromOrderAsync(Guid purchaseId, CancellationToken cancellationToken);
}
