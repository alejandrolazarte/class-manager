using ClassManager.Backoffice.Core.Operators;
using ClassManager.Security.Tokens;
using Microsoft.Extensions.Options;

namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_a_backoffice_token_has_no_platform_role;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_brands_are_forbidden(BackofficeApiFixture fixture)
{
    private const string BackofficeIssuer = "laztty-backoffice";
    private const string BranchOwnerRole = "BranchOwner";

    [Fact]
    public async Task Then_the_brands_are_forbidden_Run()
    {
        var tokenFactory = new JwtAccessTokenFactory(
            Options.Create(new JwtOptions { Issuer = BackofficeIssuer, Audience = BackofficeIssuer, SigningKey = BackofficeApiFactory.TestSigningKey }),
            fixture.ApiFactory.TimeProvider);
        var accessToken = tokenFactory.Create(new TokenSubject(Guid.CreateVersion7(), "owner@example.com", Guid.CreateVersion7(), BranchOwnerRole, BackofficeSession.Kind)).Token;
        using var client = fixture.ApiFactory.CreateHttpsClient();

        using var response = await client.GetAsync(BackofficeRequests.BrandsRoute, accessToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
