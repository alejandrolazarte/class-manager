using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Api.Authentication;

public sealed record CookieTokenResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt,
    string Kind)
{
    public static CookieTokenResponse From(TokenResponse tokens) =>
        new(tokens.AccessToken, tokens.AccessTokenExpiresAt, tokens.RefreshTokenExpiresAt, tokens.Kind);
}
