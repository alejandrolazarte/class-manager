using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.Orders;

internal static class OrderResponses
{
    public static async Task<OrderResponse> OfAsync(
        Order order,
        IClientRepository clientRepository,
        IClassGroupRepository classGroupRepository,
        CancellationToken cancellationToken)
    {
        var client = order.ClientId is { } clientId ? await clientRepository.GetByIdAsync(clientId, cancellationToken) : null;
        var classGroup = order.DeliveryClassGroupId is { } classGroupId
            ? await classGroupRepository.GetByIdAsync(classGroupId, cancellationToken)
            : null;
        return OrderResponse.From(order, client, classGroup?.Name);
    }
}
