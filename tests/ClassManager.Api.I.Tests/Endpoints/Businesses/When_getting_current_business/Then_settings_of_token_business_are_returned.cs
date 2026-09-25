using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Businesses.When_getting_current_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_settings_of_token_business_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_settings_of_token_business_are_returned_Run()
    {
        await fixture.SeedBusinessAsync();
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.GetAsync(new Uri(ApiRoutes.Business, UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var settings = await response.Content.ReadFromJsonAsync<BusinessResponse>(ApiRequests.JsonOptions);
        settings.ShouldBe(new BusinessResponse(
            business.Business.Name,
            business.Business.TimeZoneId,
            business.Business.CurrencyCode,
            business.Business.DefaultCountryCallingCode));
    }
}
