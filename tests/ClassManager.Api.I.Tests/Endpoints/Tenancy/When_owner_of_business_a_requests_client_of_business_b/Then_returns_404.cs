namespace ClassManager.Api.I.Tests.Endpoints.Tenancy.When_owner_of_business_a_requests_client_of_business_b;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var businessATokens = await client.SignUpAsync(AuthenticationRequests.SignUpCommand());
        var businessBTokens = await client.SignUpAsync(AuthenticationRequests.SignUpCommand());
        using var businessAClient = fixture.CreateClientWithToken(businessATokens.AccessToken);
        using var businessBClient = fixture.CreateClientWithToken(businessBTokens.AccessToken);
        var businessBClientRecord = await businessBClient.RegisterClientAsync();

        using var response = await businessAClient.GetAsync(new Uri($"{ApiRoutes.Clients}/{businessBClientRecord.Id}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
