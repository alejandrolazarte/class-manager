using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_searching_clients;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_current_business_clients_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_current_business_clients_are_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var ownClient = await business.HttpClient.RegisterClientAsync();
        await otherBusiness.HttpClient.RegisterClientAsync();

        var clients = await business.HttpClient.GetFromJsonAsync<List<ClientResponse>>(
            new Uri($"{ApiRoutes.Clients}?search=ana", UriKind.Relative), ApiRequests.JsonOptions);

        clients!.Select(client => client.Id).ShouldBe([ownClient.Id]);
    }
}
