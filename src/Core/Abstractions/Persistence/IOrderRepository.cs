using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IOrderRepository
{
    void Add(Order order);

    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);

    Task<Order?> GetForUpdateAsync(Guid orderId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> ListAsync(OrderSearchCriteria criteria, ClientScope? scope, CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> ListPaidBetweenOrOwingAsync(
        DateOnly firstDay, DateOnly lastDay, ClientScope? scope, CancellationToken cancellationToken);

    Task<int> CountUnpaidRequestsByClientAsync(Guid clientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> ListUnpaidRequestsCreatedBeforeAsync(DateTimeOffset createdBefore, CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> ListToDeliverInClassAsync(Guid classGroupId, CancellationToken cancellationToken);

    void RemovePayment(OrderPayment payment);

    Task<OrderPayment?> FindDeletedPaymentForUpdateAsync(Guid orderId, Guid paymentId, CancellationToken cancellationToken);

    Task<Order?> GetForUpdateByPurchaseAsync(Guid purchaseId, CancellationToken cancellationToken);
}
