namespace ClassManager.Api.I.Tests.Endpoints.Brands.When_owner_uploads_a_logo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_can_be_downloaded(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_can_be_downloaded_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        (await business.HttpClient.PutBrandLogoAsync(BrandRequests.PngLogo)).EnsureSuccessStatusCode();

        using var response = await business.HttpClient.GetBrandLogoAsync();
        (await response.Content.ReadAsByteArrayAsync()).ShouldBe(BrandRequests.PngLogo);
        response.Content.Headers.ContentType!.MediaType.ShouldBe("image/png");
        (await business.HttpClient.GetBrandAsync())!.LogoUpdatedAt.ShouldNotBeNull();
    }
}
