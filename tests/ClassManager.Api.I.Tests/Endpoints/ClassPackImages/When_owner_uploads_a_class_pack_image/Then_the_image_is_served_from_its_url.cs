namespace ClassManager.Api.I.Tests.Endpoints.ClassPackImages.When_owner_uploads_a_class_pack_image;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_image_is_served_from_its_url(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_image_is_served_from_its_url_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classPack = await business.HttpClient.CreateClassPackAsync();

        var updatedClassPack = await business.HttpClient.SetClassPackImageAsync(classPack.Id);

        updatedClassPack.ImageUrl!.ShouldContain($"/{business.Business.Id}/class-packs/{classPack.Id}/");
        using var image = await fixture.AnonymousClient.GetAsync(new Uri(updatedClassPack.ImageUrl!));
        (await image.Content.ReadAsByteArrayAsync()).ShouldBe(CatalogImageRequests.PngImage);
    }
}
