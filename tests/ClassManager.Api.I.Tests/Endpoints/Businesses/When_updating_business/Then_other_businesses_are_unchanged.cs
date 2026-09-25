using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Businesses.When_updating_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_other_businesses_are_unchanged(ApiFixture fixture)
{
    [Fact]
    public async Task Then_other_businesses_are_unchanged_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PutBusinessAsync(SettingsRequests.SettingsOf(business, SettingsRequests.MadridTimeZoneId));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var otherBusinessSettings = await otherBusiness.HttpClient.GetFromJsonAsync<BusinessResponse>(
            new Uri(ApiRoutes.Business, UriKind.Relative), ApiRequests.JsonOptions);
        otherBusinessSettings!.TimeZoneId.ShouldBe(ApiFixture.BuenosAiresTimeZoneId);
    }
}
