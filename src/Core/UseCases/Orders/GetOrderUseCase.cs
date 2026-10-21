using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Orders;

public sealed record GetOrderQuery(Guid OrderId) : IQuery;

public sealed class GetOrderUseCase(
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IClassGroupRepository classGroupRepository,
    IAccessScopes accessScopes)
    : IUseCase<GetOrderQuery, OrderResponse>
{
    public async Task<Result<OrderResponse>> ExecuteAsync(GetOrderQuery command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (order is null || !await OrderAccess.CanReachAsync(accessScopes, clientRepository, order, cancellationToken))
        {
            return OrderFailures.NotFound();
        }

        return await OrderResponses.OfAsync(order, clientRepository, classGroupRepository, cancellationToken);
    }
}
