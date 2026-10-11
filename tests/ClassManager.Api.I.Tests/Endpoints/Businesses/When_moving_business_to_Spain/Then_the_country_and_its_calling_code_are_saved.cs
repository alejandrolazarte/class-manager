using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Businesses.When_moving_business_to_Spain;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_country_and_its_calling_code_are_saved(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_country_and_its_calling_code_are_saved_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PutBusinessAsync(SettingsRequests.SpainSettingsOf(business));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        await using var context = fixture.CreateDbContext(business.Business.Id);
        var storedBusiness = await context.Businesses.AsNoTracking().SingleAsync(candidate => candidate.Id == business.Business.Id);
        storedBusiness.CountryCode.ShouldBe(ApiFixture.SpainCountryCode);
        storedBusiness.DefaultCountryCallingCode.ShouldBe("34");
        storedBusiness.Country.OwnAccountMinimumAge.ShouldBe(14);
    }
}
