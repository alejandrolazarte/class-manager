namespace ClassManager.Api.I.Tests.Endpoints.Brands.When_coach_saves_the_brand;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();

        using var response = await scenario.Coaches.Coach.PutBrandAsync(BrandRequests.DeltaBrand());

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
