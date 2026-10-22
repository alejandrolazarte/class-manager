using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IOrderRepository
{
    void Add(Order order);

    Task<Order?> GetByIdAsync(long orderId, CancellationToken cancellationToken);

    Task<Order?> GetForUpdateAsync(long orderId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> ListAsync(OrderSearchCriteria criteria, ClientScope? scope, CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> ListPaidBetweenOrOwingAsync(
        DateOnly firstDay, DateOnly lastDay, ClientScope? scope, CancellationToken cancellationToken);

    Task<int> CountUnpaidRequestsByClientAsync(long clientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> ListUnpaidRequestsCreatedBeforeAsync(DateTimeOffset createdBefore, CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> ListToDeliverInClassAsync(long classGroupId, CancellationToken cancellationToken);

    void RemovePayment(OrderPayment payment);

    Task<OrderPayment?> FindDeletedPaymentForUpdateAsync(long orderId, long paymentId, CancellationToken cancellationToken);

    Task<Order?> GetForUpdateByPurchaseAsync(long purchaseId, CancellationToken cancellationToken);
}
