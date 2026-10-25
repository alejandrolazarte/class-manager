using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Orders;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Core.UseCases.Orders;

public sealed record RecordOrderPaymentRequest(decimal? Amount, PaymentMethod? Method, DateOnly? PaidOn, bool IsDelivered = false, bool IsReady = false)
{
    public RecordOrderPaymentCommand ToCommand(long orderId) => new(orderId, Amount, Method, PaidOn, IsDelivered, IsReady);
}

public sealed record RecordOrderPaymentCommand(
    long OrderId, decimal? Amount, PaymentMethod? Method, DateOnly? PaidOn, bool IsDelivered = false, bool IsReady = false) : ICommand;

public sealed class RecordOrderPaymentUseCase(
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IClassGroupRepository classGroupRepository,
    IClassPackRepository classPackRepository,
    IClassPackPurchaseRepository purchaseRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider,
    IAccessScopes accessScopes,
    ICurrentMember currentMember,
    IOrderNotificationService orderNotifications,
    IFeatureAccess featureAccess)
    : IUseCase<RecordOrderPaymentCommand, OrderResponse>
{
    public async Task<Result<OrderResponse>> ExecuteAsync(RecordOrderPaymentCommand command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetForUpdateAsync(command.OrderId, cancellationToken);
        if (order is null || !await OrderAccess.CanReachAsync(accessScopes, clientRepository, order, cancellationToken))
        {
            return OrderFailures.NotFound();
        }

        if (await OrderAccess.PlanRefusalAsync(featureAccess, order, cancellationToken) is { } planRefusal)
        {
            return planRefusal;
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var now = timeProvider.GetUtcNow();
        var access = await currentMember.GetAccessAsync(cancellationToken);
        var payment = order.RecordPayment(command.Amount, command.Method, command.PaidOn ?? today, today, access?.UserId, now);
        if (payment.IsFailure)
        {
            return payment.Error!;
        }

        if (command.IsDelivered && order.Status == OrderStatus.Pending)
        {
            var delivery = await OrderDeliveries.DeliverAsync(
                order, classPackRepository, purchaseRepository, today, now, access?.UserId, cancellationToken);
            if (delivery.IsFailure)
            {
                return delivery.Error!;
            }
        }
        else if (command.IsReady && order.AwaitsPickup && !order.IsReady)
        {
            order.MarkReady(now);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        if (order.PaymentStatus == OrderPaymentStatus.Paid)
        {
            await orderNotifications.OrderPaidAsync(order, cancellationToken);
        }

        return await OrderResponses.OfAsync(order, clientRepository, classGroupRepository, cancellationToken);
    }
}
