using System.Globalization;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Security.Tokens;

namespace ClassManager.Infrastructure.Security;

internal sealed class SessionUserClaims(IUserRepository userRepository)
{
    public async Task<TokenSubject> AddToAsync(TokenSubject subject, CancellationToken cancellationToken) =>
        await userRepository.FindIdByAccountAsync(subject.TenantId, subject.UserId, cancellationToken) is { } userId
            ? subject with { Claims = new Dictionary<string, string> { [SessionClaimTypes.UserId] = userId.ToString(CultureInfo.InvariantCulture) } }
            : subject;
}
