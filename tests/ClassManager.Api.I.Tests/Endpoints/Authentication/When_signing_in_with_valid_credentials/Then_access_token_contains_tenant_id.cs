using ClassManager.Security.Tokens;
using ClassManager.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_in_with_valid_credentials;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_access_token_contains_tenant_id(ApiFixture fixture)
{
    [Fact]
    public async Task Then_access_token_contains_tenant_id_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        await client.SignUpAsync(command);
        await using var context = fixture.CreateDbContext(Guid.Empty);
        var business = await context.Businesses.SingleAsync(business => business.Name == command.BusinessName);

        var tokens = await client.SignInAsync(command.Email!);

        var accessToken = new JsonWebToken(tokens.AccessToken);
        accessToken.GetClaim(TenantClaimTypes.TenantId).Value.ShouldBe(business.Id.ToString());
        accessToken.GetClaim(SecurityClaimTypes.Email).Value.ShouldBe(command.Email);
        accessToken.GetClaim(SecurityClaimTypes.Role).Value.ShouldBe("BranchOwner");
    }
}
