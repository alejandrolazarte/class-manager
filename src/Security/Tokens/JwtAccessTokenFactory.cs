using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ClassManager.Security.Tokens;

internal sealed class JwtAccessTokenFactory(IOptions<JwtOptions> options, TimeProvider timeProvider) : IAccessTokenFactory
{
    private readonly JsonWebTokenHandler _tokenHandler = new() { SetDefaultTimesOnTokenCreation = false };

    public AccessToken Create(TokenSubject subject)
    {
        var jwtOptions = options.Value;
        var issuedAt = TruncateToSeconds(timeProvider.GetUtcNow());
        var expiresAt = issuedAt + jwtOptions.AccessTokenLifetime;

        var token = _tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            Claims = new Dictionary<string, object>
            {
                [SecurityClaimTypes.Subject] = subject.UserId.ToString(),
                [SecurityClaimTypes.Email] = subject.Email,
                [jwtOptions.TenantClaimType] = subject.TenantId.ToString(),
                [SecurityClaimTypes.Role] = subject.Role,
            },
            IssuedAt = issuedAt.UtcDateTime,
            NotBefore = issuedAt.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = new SigningCredentials(jwtOptions.CreateSigningKey(), SecurityAlgorithms.HmacSha256),
        });

        return new AccessToken(token, expiresAt);
    }

    private static DateTimeOffset TruncateToSeconds(DateTimeOffset value) =>
        new(value.Ticks - (value.Ticks % TimeSpan.TicksPerSecond), value.Offset);
}
