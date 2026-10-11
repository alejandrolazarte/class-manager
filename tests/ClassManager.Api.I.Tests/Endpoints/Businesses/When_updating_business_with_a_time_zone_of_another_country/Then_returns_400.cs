namespace ClassManager.Api.I.Tests.Endpoints.Businesses.When_updating_business_with_a_time_zone_of_another_country;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PutBusinessAsync(SettingsRequests.SettingsOf(business, SettingsRequests.MadridTimeZoneId));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
