namespace ClassManager.Api.I.Tests.Endpoints.FamilyPush.When_an_order_is_ready;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_family_gets_a_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_family_gets_a_push_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        (await scenario.Family.PostFamilyOrderForClassAsync(
            scenario.Coaches.CoachClassGroup.Id, FamilyShopRequests.ProductLine(product.Variants[0].Id, 1))).EnsureSuccessStatusCode();
        var orderId = (await scenario.Family.ListFamilyOrdersAsync()).Single().Id;
        (await owner.PutOrderPaymentAsync(orderId)).EnsureSuccessStatusCode();

        (await owner.PutOrderReadyAsync(orderId)).EnsureSuccessStatusCode();

        var push = await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint);
        push.Title.ShouldBe("Tu pedido está listo");
        push.Url.ShouldBe("/family/orders");
    }
}
