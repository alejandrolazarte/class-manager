using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Security.Tokens;

namespace ClassManager.Infrastructure.Security;

internal sealed class TokenService(ITokenIssuer tokenIssuer) : ITokenService
{
    public async Task<IssuedTokens> IssueAsync(SessionUser user, CancellationToken cancellationToken)
    {
        var tokens = await tokenIssuer.IssueAsync(ToTokenSubject(user), cancellationToken);

        return ToIssuedTokens(tokens);
    }

    public async Task<Result<IssuedTokens>> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var tokens = await tokenIssuer.RefreshAsync(refreshToken, null, cancellationToken);

        return tokens is null
            ? Result.Unauthorized<IssuedTokens>(AuthenticationErrorCodes.InvalidRefreshTokenMessage, AuthenticationErrorCodes.InvalidRefreshToken)
            : ToIssuedTokens(tokens);
    }

    public async Task<Result<IssuedTokens>> SwitchBusinessAsync(string refreshToken, Guid businessId, string? kind, CancellationToken cancellationToken)
    {
        var tokens = await tokenIssuer.SwitchAsync(refreshToken, businessId, kind, cancellationToken);

        return tokens is null
            ? Result.Unauthorized<IssuedTokens>(AuthenticationErrorCodes.InvalidRefreshTokenMessage, AuthenticationErrorCodes.InvalidRefreshToken)
            : ToIssuedTokens(tokens);
    }

    public Task RevokeAsync(string refreshToken, CancellationToken cancellationToken) =>
        tokenIssuer.RevokeAsync(refreshToken, cancellationToken);

    private static TokenSubject ToTokenSubject(SessionUser user) =>
        new(user.UserId, user.Email, user.TenantId, user.RoleName, user.Kind);

    private static IssuedTokens ToIssuedTokens(IssuedTokenPair tokens) =>
        new(tokens.AccessToken, tokens.AccessTokenExpiresAt, tokens.RefreshToken, tokens.RefreshTokenExpiresAt, tokens.Kind);
}
