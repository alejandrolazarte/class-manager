using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.Abstractions.Notifications;

public interface IOrderNotificationService
{
    Task OrderPlacedAsync(Order order, CancellationToken cancellationToken);

    Task OrderPaidAsync(Order order, CancellationToken cancellationToken);

    Task OrderReadyAsync(Order order, CancellationToken cancellationToken);

    Task OrderCancelledAsync(Order order, OrderCancellationReason reason, CancellationToken cancellationToken);
}
