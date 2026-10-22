using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Orders;

public sealed record DeleteOrderPaymentCommand(Guid OrderId, Guid PaymentId) : ICommand;

public sealed class DeleteOrderPaymentUseCase(
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IClassGroupRepository classGroupRepository,
    IUnitOfWork unitOfWork,
    IAccessScopes accessScopes,
    ICurrentMember currentMember)
    : IUseCase<DeleteOrderPaymentCommand, OrderResponse>
{
    public async Task<Result<OrderResponse>> ExecuteAsync(DeleteOrderPaymentCommand command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetForUpdateAsync(command.OrderId, cancellationToken);
        if (order is null || !await OrderAccess.CanReachAsync(accessScopes, clientRepository, order, cancellationToken))
        {
            return OrderFailures.NotFound();
        }

        var deletion = order.DeletePayment(command.PaymentId);
        if (deletion.IsFailure)
        {
            return deletion.Error!;
        }

        if (!await OrderAccess.CanUndoPaymentAsync(currentMember, deletion.Value!, cancellationToken))
        {
            return AccessRules.NotYours();
        }

        orderRepository.RemovePayment(deletion.Value!);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await OrderResponses.OfAsync(order, clientRepository, classGroupRepository, cancellationToken);
    }
}
