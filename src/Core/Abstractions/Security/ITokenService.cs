using ClassManager.Core.Common;

namespace ClassManager.Core.Abstractions.Security;

public interface ITokenService
{
    Task<IssuedTokens> IssueAsync(SessionUser user, CancellationToken cancellationToken);

    Task<Result<IssuedTokens>> RefreshAsync(string refreshToken, CancellationToken cancellationToken);

    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken);
}
