using ClassManager.Backoffice.Core.Operators;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Security.Tokens;

namespace ClassManager.Backoffice.Infrastructure.Operators;

internal sealed class BackofficeTokenSubjectResolver(
    IBackofficeLoginRepository loginRepository,
    IPlatformOperatorDirectory operatorDirectory)
    : ITokenSubjectResolver
{
    public async Task<TokenSubject?> ResolveAsync(Guid userId, string email, Guid? tenantId, string? kind, CancellationToken cancellationToken)
    {
        if (kind != BackofficeSession.Kind || !await loginRepository.HasEnabledLoginAsync(userId, cancellationToken))
        {
            return null;
        }

        var platformOperator = await operatorDirectory.FindAsync(userId, cancellationToken);
        if (platformOperator is null || (tenantId is { } requestedTenantId && requestedTenantId != platformOperator.BusinessId))
        {
            return null;
        }

        return new TokenSubject(
            userId,
            email,
            platformOperator.BusinessId,
            platformOperator.Role.ToString(),
            BackofficeSession.Kind,
            new Dictionary<string, string> { [SessionClaimTypes.UserId] = platformOperator.UserId.ToString() });
    }
}
