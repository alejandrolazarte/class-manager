using ClassManager.Core.UseCases.Countries;

namespace ClassManager.Api.I.Tests.Endpoints.Countries.When_listing_countries_without_signing_in;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_Spain_has_its_currency_and_time_zones(ApiFixture fixture)
{
    [Fact]
    public async Task Then_Spain_has_its_currency_and_time_zones_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();

        var countries = await anonymous.GetFromJsonAsync<List<CountryResponse>>(
            new Uri(ApiRoutes.Countries, UriKind.Relative), ApiRequests.JsonOptions);

        countries.ShouldNotBeNull();
        var spain = countries.Single(country => country.Code == ApiFixture.SpainCountryCode);
        spain.TimeZoneIds.ShouldBe([SettingsRequests.MadridTimeZoneId, "Atlantic/Canary"]);
        spain.CurrencyCode.ShouldBe(SettingsRequests.EuroCurrencyCode);
        spain.CallingCode.ShouldBe("34");
    }
}
