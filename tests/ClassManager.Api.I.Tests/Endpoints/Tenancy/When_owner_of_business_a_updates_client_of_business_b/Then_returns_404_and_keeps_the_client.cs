using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.Tenancy.When_owner_of_business_a_updates_client_of_business_b;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404_and_keeps_the_client(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_and_keeps_the_client_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var businessATokens = await client.SignUpAsync(AuthenticationRequests.SignUpCommand());
        var businessBTokens = await client.SignUpAsync(AuthenticationRequests.SignUpCommand());
        using var businessAClient = fixture.CreateClientWithToken(businessATokens.AccessToken);
        using var businessBClient = fixture.CreateClientWithToken(businessBTokens.AccessToken);
        var businessBClientRecord = await businessBClient.RegisterClientAsync();

        using var response = await businessAClient.PutClientAsync(
            businessBClientRecord.Id,
            new UpdateClientRequest("Someone else", "11 5566-7788", null, null));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var storedClient = await businessBClient.GetFromJsonAsync<ClientResponse>(
            new Uri($"{ApiRoutes.Clients}/{businessBClientRecord.Id}", UriKind.Relative), ApiRequests.JsonOptions);
        storedClient!.FullName.ShouldBe(businessBClientRecord.FullName);
    }
}
