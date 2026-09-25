namespace ClassManager.Api.I.Tests.Endpoints.Tenancy.When_token_is_signed_with_another_key;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_401(ApiFixture fixture)
{
    private const string AnotherSigningKey = "another-signing-key-that-is-long-enough-to-be-valid";

    [Fact]
    public async Task Then_returns_401_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        using var client = fixture.CreateClientWithToken(fixture.CreateAccessToken(business.Business.Id, signingKey: AnotherSigningKey));

        using var response = await client.GetAsync(new Uri(ApiRoutes.Clients, UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
