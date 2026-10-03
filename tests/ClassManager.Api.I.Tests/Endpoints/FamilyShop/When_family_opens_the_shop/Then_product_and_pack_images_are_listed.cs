namespace ClassManager.Api.I.Tests.Endpoints.FamilyShop.When_family_opens_the_shop;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_product_and_pack_images_are_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_product_and_pack_images_are_listed_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var product = await owner.SetProductImageAsync((await owner.CreateProductAsync()).Id);
        var classPack = await owner.SetClassPackImageAsync((await owner.CreateClassPackAsync()).Id);

        var shop = await scenario.Family.GetFamilyShopAsync();

        shop.Products.Single().ImageUrl.ShouldBe(product.ImageUrl);
        shop.Packs.Single().ImageUrl.ShouldBe(classPack.ImageUrl);
    }
}
