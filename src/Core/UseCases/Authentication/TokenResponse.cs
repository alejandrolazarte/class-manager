using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record TokenResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt)
{
    public static TokenResponse From(IssuedTokens tokens) =>
        new(tokens.AccessToken, tokens.AccessTokenExpiresAt, tokens.RefreshToken, tokens.RefreshTokenExpiresAt);
}
