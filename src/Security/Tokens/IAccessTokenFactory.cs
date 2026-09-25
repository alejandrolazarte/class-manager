namespace ClassManager.Security.Tokens;

internal sealed record AccessToken(string Token, DateTimeOffset ExpiresAt);

internal interface IAccessTokenFactory
{
    AccessToken Create(TokenSubject subject);
}
