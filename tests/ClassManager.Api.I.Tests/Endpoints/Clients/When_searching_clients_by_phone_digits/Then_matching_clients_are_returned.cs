using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_searching_clients_by_phone_digits;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_matching_clients_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_matching_clients_are_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var matchingClient = await business.HttpClient.RegisterClientAsync();
        await business.HttpClient.RegisterClientAsync("Bruno Díaz", "351 555-0000");

        var clients = await business.HttpClient.GetFromJsonAsync<List<ClientResponse>>(
            new Uri($"{ApiRoutes.Clients}?search=11%2022", UriKind.Relative), ApiRequests.JsonOptions);

        clients!.Select(client => client.Id).ShouldBe([matchingClient.Id]);
    }
}
