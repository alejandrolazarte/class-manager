using System.Security.Claims;
using System.Text;
using ClassManager.Security.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ClassManager.Api.I.Tests.Endpoints.Tenancy.When_token_has_no_tenant_claim;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_401(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_401_Run()
    {
        var jwtOptions = fixture.ApiFactory.Services.GetRequiredService<IOptions<JwtOptions>>().Value;
        var token = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            Subject = new ClaimsIdentity([new Claim(SecurityClaimTypes.Subject, Guid.CreateVersion7().ToString())]),
            IssuedAt = BusinessApiFactory.Now.UtcDateTime,
            NotBefore = BusinessApiFactory.Now.UtcDateTime,
            Expires = BusinessApiFactory.Now.AddMinutes(15).UtcDateTime,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(BusinessApiFactory.TestSigningKey)),
                SecurityAlgorithms.HmacSha256),
        });
        using var client = fixture.CreateClientWithToken(token);

        using var response = await client.GetAsync(new Uri(ApiRoutes.Clients, UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
