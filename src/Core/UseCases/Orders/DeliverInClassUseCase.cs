using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.Orders;

public sealed record DeliverInClassCommand(Guid ClassGroupId, Guid OrderId) : ICommand;

public sealed class DeliverInClassUseCase(
    IClassGroupRepository classGroupRepository,
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork,
    IAccessScopes accessScopes,
    ICurrentMember currentMember,
    TimeProvider timeProvider)
    : IUseCase<DeliverInClassCommand, ClassDeliveryResponse>
{
    public async Task<Result<ClassDeliveryResponse>> ExecuteAsync(DeliverInClassCommand command, CancellationToken cancellationToken)
    {
        var classGroup = await ClassDeliveries.GetClassGroupInScopeAsync(command.ClassGroupId, classGroupRepository, accessScopes, cancellationToken);
        if (classGroup.IsFailure)
        {
            return classGroup.Error!;
        }

        var order = await orderRepository.GetForUpdateAsync(command.OrderId, cancellationToken);
        if (order is null || order.Delivery != DeliveryMethod.InClass || order.DeliveryClassGroupId != classGroup.Value!.Id)
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

        var client = order.ClientId is { } clientId ? await clientRepository.GetByIdAsync(clientId, cancellationToken) : null;
        return ClassDeliveries.ResponseOf(order, client?.FullName);
    }
}
