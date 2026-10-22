using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Core.UseCases.Fees;

internal static class MoneyRules
{
    public static Task<bool> CanCollectFromAsync(
        IAccessScopes accessScopes,
        IClientRepository clientRepository,
        long clientId,
        CancellationToken cancellationToken) =>
        AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [clientId], Permissions.Payments.ViewAll, cancellationToken);

    public static async Task<bool> CanUndoAsync(
        IAccessScopes accessScopes,
        IClientRepository clientRepository,
        ICurrentMember currentMember,
        long clientId,
        long? recordedByUserId,
        CancellationToken cancellationToken)
    {
        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (access is null)
        {
            return false;
        }

        return access.HasPermission(Permissions.Payments.ViewAll)
            || (recordedByUserId is not null && recordedByUserId == access.UserId && await CanCollectFromAsync(accessScopes, clientRepository, clientId, cancellationToken));
    }
}
