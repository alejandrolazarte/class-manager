namespace ClassManager.Api.I.Tests.Endpoints.Brands.When_logo_is_not_an_image;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PutBrandLogoAsync("not an image"u8.ToArray());

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
