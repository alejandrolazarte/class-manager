using ClassManager.Core.UseCases.Families;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyNews.When_an_order_of_the_family_is_ready;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_news_points_to_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_news_points_to_it_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        (await scenario.Family.PostFamilyOrderAsync(FamilyShopRequests.ProductLine(product.Variants[0].Id, 1))).EnsureSuccessStatusCode();
        var orderId = (await scenario.Family.ListFamilyOrdersAsync()).Single().Id;
        (await owner.PutOrderPaymentAsync(orderId)).EnsureSuccessStatusCode();
        (await owner.PutOrderReadyAsync(orderId)).EnsureSuccessStatusCode();

        var news = await scenario.Family.GetFamilyNewsAsync();

        news!.Items.Single(item => item.Kind == FamilyNewsKind.OrderReady).OrderId.ShouldBe(orderId);
    }
}
