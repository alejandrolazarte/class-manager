namespace ClassManager.Api.I.Tests.Endpoints.Brands.When_brand_color_is_malformed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PutBrandAsync(BrandRequests.DeltaBrand() with { ThemeColor = "azul" });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
