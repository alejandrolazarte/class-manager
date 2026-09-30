namespace ClassManager.Api.I.Tests.Endpoints.Brands.When_owner_removes_the_logo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_found(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_found_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        (await business.HttpClient.PutBrandLogoAsync(BrandRequests.PngLogo)).EnsureSuccessStatusCode();

        (await business.HttpClient.DeleteBrandLogoAsync()).EnsureSuccessStatusCode();

        using var response = await business.HttpClient.GetBrandLogoAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await business.HttpClient.GetBrandAsync())!.LogoUpdatedAt.ShouldBeNull();
    }
}
