using System.Text.Json;

using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Businesses.When_updating_business_with_a_link_that_is_not_Google_Maps;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    private const string ErrorsProperty = "errors";

    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PutBusinessAsync(
            SettingsRequests.SettingsOf(business, locationName: "Av. Costanera 1450", locationMapsUrl: "https://www.example.com/sede"));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty(ErrorsProperty)
            .TryGetProperty(JsonNamingPolicy.CamelCase.ConvertName(nameof(UpdateBusinessSettingsCommand.LocationMapsUrl)), out _)
            .ShouldBeTrue();
    }
}
