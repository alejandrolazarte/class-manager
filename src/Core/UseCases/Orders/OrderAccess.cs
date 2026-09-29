using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.Orders;

internal static class OrderAccess
{
    public static async Task<bool> CanReachAsync(
        IAccessScopes accessScopes,
        IClientRepository clientRepository,
        Order order,
        CancellationToken cancellationToken)
    {
        var scope = await accessScopes.ForClientsAsync(Permissions.Orders.ViewAll, cancellationToken);
        if (scope is null)
        {
            return true;
        }

        return order.ClientId is { } clientId && await clientRepository.IsInScopeAsync(clientId, scope, cancellationToken);
    }
}
