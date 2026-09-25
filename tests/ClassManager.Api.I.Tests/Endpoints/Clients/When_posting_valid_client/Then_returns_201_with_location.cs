using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_posting_valid_client;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_201_with_location(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_201_with_location_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostClientAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var client = await response.Content.ReadFromJsonAsync<ClientResponse>(ApiRequests.JsonOptions);
        client!.PhoneNumber.ShouldBe(ApiRequests.NormalizedClientPhoneNumber);
        response.Headers.Location!.ToString().ShouldBe($"{ApiRoutes.Clients}/{client.Id}");
    }
}
