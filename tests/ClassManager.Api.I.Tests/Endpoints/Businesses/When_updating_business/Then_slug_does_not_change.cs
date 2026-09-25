using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Businesses.When_updating_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_slug_does_not_change(ApiFixture fixture)
{
    private const string NewBusinessName = "Panadería Renovada";

    [Fact]
    public async Task Then_slug_does_not_change_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PutBusinessAsync(SettingsRequests.SettingsOf(business, name: NewBusinessName));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        await using var context = fixture.CreateDbContext(business.Business.Id);
        var storedBusiness = await context.Businesses.AsNoTracking().SingleAsync(candidate => candidate.Id == business.Business.Id);
        storedBusiness.Slug.ShouldBe(business.Business.Slug);
        storedBusiness.Name.ShouldBe(NewBusinessName);
    }
}
