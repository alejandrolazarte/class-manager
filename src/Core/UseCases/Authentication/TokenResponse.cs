using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record TokenResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    string Kind = AccountKinds.Team)
{
    public static TokenResponse From(IssuedTokens tokens) =>
        new(
            tokens.AccessToken,
            tokens.AccessTokenExpiresAt,
            tokens.RefreshToken,
            tokens.RefreshTokenExpiresAt,
            tokens.Kind ?? AccountKinds.Team);
}
