using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Orders;

public sealed record MarkOrderDeliveredCommand(Guid OrderId) : ICommand;

public sealed class MarkOrderDeliveredUseCase(
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IClassGroupRepository classGroupRepository,
    IUnitOfWork unitOfWork,
    IAccessScopes accessScopes,
    ICurrentMember currentMember,
    TimeProvider timeProvider)
    : IUseCase<MarkOrderDeliveredCommand, OrderResponse>
{
    public async Task<Result<OrderResponse>> ExecuteAsync(MarkOrderDeliveredCommand command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetForUpdateAsync(command.OrderId, cancellationToken);
        if (order is null || !await OrderAccess.CanReachAsync(accessScopes, clientRepository, order, cancellationToken))
        {
            return OrderFailures.NotFound();
        }

        var access = await currentMember.GetAccessAsync(cancellationToken);
        var delivery = order.MarkDelivered(access?.UserId, timeProvider.GetUtcNow());
        if (delivery.IsFailure)
        {
            return delivery.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await OrderResponses.OfAsync(order, clientRepository, classGroupRepository, cancellationToken);
    }
}
