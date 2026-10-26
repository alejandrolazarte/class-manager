using System.Globalization;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Security.Tokens;

namespace ClassManager.Infrastructure.Security;

internal sealed class SessionUserClaims(IUserRepository userRepository, IBusinessRepository businessRepository)
{
    public async Task<TokenSubject> AddToAsync(TokenSubject subject, CancellationToken cancellationToken)
    {
        var claims = new Dictionary<string, string>();
        if (await userRepository.FindIdByAccountAsync(subject.TenantId, subject.UserId, cancellationToken) is { } userId)
        {
            claims[SessionClaimTypes.UserId] = userId.ToString(CultureInfo.InvariantCulture);
        }

        if (await businessRepository.FindOrganizationIdAsync(subject.TenantId, cancellationToken) is { } organizationId)
        {
            claims[SessionClaimTypes.OrganizationId] = organizationId.ToString(CultureInfo.InvariantCulture);
        }

        return claims.Count == 0 ? subject : subject with { Claims = claims };
    }
}
