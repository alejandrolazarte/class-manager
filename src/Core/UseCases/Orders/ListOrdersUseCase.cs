using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Core.UseCases.Orders;

public sealed record ListOrdersQuery(Guid? ClientId, bool AwaitingPickupOnly, bool RequestedOnly = false);

public sealed class ListOrdersUseCase(
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IClassGroupRepository classGroupRepository,
    IAccessScopes accessScopes)
    : IUseCase<ListOrdersQuery, IReadOnlyList<OrderResponse>>
{
    private const int OrderLimit = 200;

    public async Task<Result<IReadOnlyList<OrderResponse>>> ExecuteAsync(ListOrdersQuery command, CancellationToken cancellationToken)
    {
        var scope = await accessScopes.ForClientsAsync(Permissions.Orders.ViewAll, cancellationToken);
        var orders = await orderRepository.ListAsync(
            new OrderSearchCriteria(command.ClientId, command.AwaitingPickupOnly, OrderLimit, command.RequestedOnly), scope, cancellationToken);
        var clientIds = orders.Select(order => order.ClientId).OfType<Guid>().Distinct().ToList();
        var clientsById = (await clientRepository.ListByIdsAsync(clientIds, cancellationToken)).ToDictionary(client => client.Id);
        var classGroupNames = orders.Any(order => order.DeliveryClassGroupId is not null)
            ? (await classGroupRepository.ListAllAsync(cancellationToken)).ToDictionary(classGroup => classGroup.Id, classGroup => classGroup.Name)
            : [];

        return Result.Success<IReadOnlyList<OrderResponse>>(
        [
            .. orders.Select(order => OrderResponse.From(
                order,
                order.ClientId is { } clientId ? clientsById.GetValueOrDefault(clientId) : null,
                order.DeliveryClassGroupId is { } classGroupId ? classGroupNames.GetValueOrDefault(classGroupId) : null)),
        ]);
    }
}
