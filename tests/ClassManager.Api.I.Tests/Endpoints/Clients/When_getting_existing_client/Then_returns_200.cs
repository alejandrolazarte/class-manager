using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_getting_existing_client;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_200(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_200_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var registeredClient = await business.HttpClient.RegisterClientAsync();

        var client = await business.HttpClient.GetFromJsonAsync<ClientResponse>(
            new Uri($"{ApiRoutes.Clients}/{registeredClient.Id}", UriKind.Relative), ApiRequests.JsonOptions);

        client.ShouldBe(registeredClient);
    }
}
