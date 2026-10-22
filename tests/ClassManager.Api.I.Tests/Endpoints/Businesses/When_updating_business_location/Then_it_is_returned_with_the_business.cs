namespace ClassManager.Api.I.Tests.Endpoints.Businesses.When_updating_business_location;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_returned_with_the_business(ApiFixture fixture)
{
    private const string LocationName = "Av. Costanera 1450, Ciudad";
    private const string LocationMapsUrl = "https://maps.app.goo.gl/k3Hq8vPzR2";

    [Fact]
    public async Task Then_it_is_returned_with_the_business_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PutBusinessAsync(
            SettingsRequests.SettingsOf(business, locationName: LocationName, locationMapsUrl: LocationMapsUrl));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var settings = await business.HttpClient.GetCurrentBusinessAsync();
        settings!.LocationName.ShouldBe(LocationName);
        settings.LocationMapsUrl.ShouldBe(LocationMapsUrl);
    }
}
