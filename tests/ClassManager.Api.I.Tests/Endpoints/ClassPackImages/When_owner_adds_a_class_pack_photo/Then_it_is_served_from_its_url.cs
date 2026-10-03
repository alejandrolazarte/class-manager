namespace ClassManager.Api.I.Tests.Endpoints.ClassPackImages.When_owner_adds_a_class_pack_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_served_from_its_url(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_served_from_its_url_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classPack = await business.HttpClient.CreateClassPackAsync();

        var image = (await business.HttpClient.AddClassPackImageAsync(classPack.Id)).Images.Single();

        image.Url.ShouldContain($"/{business.Business.Id}/class-packs/{classPack.Id}/");
        using var response = await fixture.AnonymousClient.GetAsync(new Uri(image.Url));
        (await response.Content.ReadAsByteArrayAsync()).ShouldBe(CatalogImageRequests.PngImage);
    }
}
