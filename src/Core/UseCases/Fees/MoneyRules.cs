using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Core.UseCases.Fees;

internal static class MoneyRules
{
    public static Task<bool> CanCollectFromAsync(
        IAccessScopes accessScopes,
        IClientRepository clientRepository,
        Guid clientId,
        CancellationToken cancellationToken) =>
        AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [clientId], Permissions.Payments.ViewAll, cancellationToken);

    public static async Task<bool> CanUndoAsync(
        IAccessScopes accessScopes,
        IClientRepository clientRepository,
        ICurrentMember currentMember,
        Guid clientId,
        Guid? recordedByUserId,
        CancellationToken cancellationToken)
    {
        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (access is null)
        {
            return false;
        }

        return access.HasPermission(Permissions.Payments.ViewAll)
            || (recordedByUserId == access.UserId && await CanCollectFromAsync(accessScopes, clientRepository, clientId, cancellationToken));
    }
}
