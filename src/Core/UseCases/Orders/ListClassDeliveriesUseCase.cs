using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Orders;

public sealed record ListClassDeliveriesQuery(Guid ClassGroupId);

public sealed class ListClassDeliveriesUseCase(
    IClassGroupRepository classGroupRepository,
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IAccessScopes accessScopes)
    : IUseCase<ListClassDeliveriesQuery, IReadOnlyList<ClassDeliveryResponse>>
{
    public async Task<Result<IReadOnlyList<ClassDeliveryResponse>>> ExecuteAsync(ListClassDeliveriesQuery command, CancellationToken cancellationToken)
    {
        var classGroup = await ClassDeliveries.GetClassGroupInScopeAsync(command.ClassGroupId, classGroupRepository, accessScopes, cancellationToken);
        if (classGroup.IsFailure)
        {
            return classGroup.Error!;
        }

        var orders = await orderRepository.ListToDeliverInClassAsync(classGroup.Value!.Id, cancellationToken);
        var clientNames = (await clientRepository.ListByIdsAsync([.. orders.Select(order => order.ClientId).OfType<Guid>().Distinct()], cancellationToken))
            .ToDictionary(client => client.Id, client => client.FullName);
        return Result.Success<IReadOnlyList<ClassDeliveryResponse>>(
        [
            .. orders.Select(order => ClassDeliveries.ResponseOf(
                order, order.ClientId is { } clientId ? clientNames.GetValueOrDefault(clientId) : null)),
        ]);
    }
}
