namespace ClassManager.Api.I.Tests.Endpoints.FamilyShop.When_family_opens_the_shop;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_product_and_pack_photos_are_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_product_and_pack_photos_are_listed_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var product = await owner.AddProductImageAsync((await owner.CreateProductAsync()).Id);
        var classPack = await owner.AddClassPackImageAsync((await owner.CreateClassPackAsync()).Id);

        var shop = await scenario.Family.GetFamilyShopAsync();

        shop.Products.Single().Images.ShouldBe(product.Images);
        shop.Packs.Single().Images.ShouldBe(classPack.Images);
    }
}
