using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases;

internal static class AccessRules
{
    public static ResultError NotYours() => new(MemberErrorCodes.NotYours, MemberErrorCodes.NotYoursMessage, ErrorKind.Forbidden);

    public static Task<bool> CanReachClientsAsync(
        IAccessScopes accessScopes,
        IClientRepository clientRepository,
        IEnumerable<Guid> clientIds,
        CancellationToken cancellationToken) =>
        CanReachClientsAsync(accessScopes, clientRepository, clientIds, Permissions.Students.ViewAll, cancellationToken);

    public static async Task<bool> CanReachClientsAsync(
        IAccessScopes accessScopes,
        IClientRepository clientRepository,
        IEnumerable<Guid> clientIds,
        string everyClientPermission,
        CancellationToken cancellationToken)
    {
        var scope = await accessScopes.ForClientsAsync(everyClientPermission, cancellationToken);
        if (scope is null)
        {
            return true;
        }

        foreach (var clientId in clientIds.Distinct())
        {
            if (!await clientRepository.IsInScopeAsync(clientId, scope, cancellationToken))
            {
                return false;
            }
        }

        return true;
    }
}
