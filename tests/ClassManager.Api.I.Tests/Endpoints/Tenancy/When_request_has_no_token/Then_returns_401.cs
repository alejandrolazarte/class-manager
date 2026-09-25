namespace ClassManager.Api.I.Tests.Endpoints.Tenancy.When_request_has_no_token;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_401(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_401_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();

        using var response = await client.PostClientAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
