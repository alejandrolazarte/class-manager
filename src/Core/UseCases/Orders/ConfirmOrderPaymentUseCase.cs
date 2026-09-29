using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Orders;

public sealed record ConfirmOrderPaymentRequest(PaymentMethod? Method, DateOnly? PaidOn)
{
    public ConfirmOrderPaymentCommand ToCommand(Guid orderId) => new(orderId, Method, PaidOn);
}

public sealed record ConfirmOrderPaymentCommand(Guid OrderId, PaymentMethod? Method, DateOnly? PaidOn);

public sealed class ConfirmOrderPaymentUseCase(
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IClassPackRepository classPackRepository,
    IClassPackPurchaseRepository purchaseRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider,
    IAccessScopes accessScopes,
    ICurrentMember currentMember)
    : IUseCase<ConfirmOrderPaymentCommand, OrderResponse>
{
    public async Task<Result<OrderResponse>> ExecuteAsync(ConfirmOrderPaymentCommand command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetForUpdateAsync(command.OrderId, cancellationToken);
        if (order is null || !await OrderAccess.CanReachAsync(accessScopes, clientRepository, order, cancellationToken))
        {
            return OrderFailures.NotFound();
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var access = await currentMember.GetAccessAsync(cancellationToken);
        var payment = order.ConfirmPayment(command.Method, command.PaidOn ?? today, today, access?.UserId);
        if (payment.IsFailure)
        {
            return payment.Error!;
        }

        var packs = await OrderDrafts.LoadPacksAsync(order.Lines.Select(line => line.ClassPackId), classPackRepository, cancellationToken);
        var credit = OrderPayments.CreditClassPacks(order, packs, purchaseRepository, today, timeProvider.GetUtcNow(), access?.UserId);
        if (credit.IsFailure)
        {
            return credit.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var client = order.ClientId is { } clientId ? await clientRepository.GetByIdAsync(clientId, cancellationToken) : null;
        return OrderResponse.From(order, client);
    }
}
