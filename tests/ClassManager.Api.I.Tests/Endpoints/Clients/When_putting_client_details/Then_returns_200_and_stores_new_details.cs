using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_putting_client_details;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_200_and_stores_new_details(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_200_and_stores_new_details_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var registeredClient = await business.HttpClient.RegisterClientAsync();

        using var response = await business.HttpClient.PutClientAsync(
            registeredClient.Id,
            new UpdateClientRequest("Ana María Pérez", "11 5566-7788", "ana@example.com", "Allergic to chlorine"));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var storedClient = await business.HttpClient.GetFromJsonAsync<ClientResponse>(
            new Uri($"{ApiRoutes.Clients}/{registeredClient.Id}", UriKind.Relative), ApiRequests.JsonOptions);
        storedClient.ShouldBe(new ClientResponse(
            registeredClient.Id,
            "Ana María Pérez",
            "+541155667788",
            "ana@example.com",
            "Allergic to chlorine",
            registeredClient.CreatedAt));
    }
}
