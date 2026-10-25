using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

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

    public static async Task<bool> CanUndoPaymentAsync(ICurrentMember currentMember, OrderPayment payment, CancellationToken cancellationToken)
    {
        var access = await currentMember.GetAccessAsync(cancellationToken);
        return access is not null
            && (access.HasPermission(Permissions.Payments.ViewAll) || (payment.RecordedByUserId is not null && payment.RecordedByUserId == access.UserId));
    }

    public static async Task<ResultError?> PlanRefusalAsync(IFeatureAccess featureAccess, Order order, CancellationToken cancellationToken)
    {
        var featureCode = order.HasProducts ? Features.Shop : Features.ClassPacks;
        var features = await featureAccess.GetCurrentAsync(cancellationToken);
        return features.Has(featureCode) ? null : OrderFailures.NotInPlan(featureCode);
    }
}
