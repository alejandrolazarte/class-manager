using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyShop.When_branch_confirms_payment_of_a_family_order;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_classes_are_credited_and_products_await_pickup(ApiFixture fixture)
{
    [Fact]
    public async Task Then_classes_are_credited_and_products_await_pickup_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var classPack = await owner.CreateClassPackAsync();
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        var order = await scenario.Family.PlaceFamilyOrderAsync(
            FamilyShopRequests.PackLine(classPack.Id), FamilyShopRequests.ProductLine(product.Variants[0].Id, 1));
        (await owner.ListOrdersAsync()).Single().Id.ShouldBe(order.Id);

        using var response = await owner.PutOrderPaymentAsync(order.Id);

        (await response.Content.ReadFromJsonAsync<OrderResponse>(ApiRequests.JsonOptions))!.AwaitsPickup.ShouldBeTrue();
        (await owner.GetClassBalanceAsync(scenario.FamilyId))!.AvailableClasses.ShouldBe(4);
        (await owner.ListProductsAsync()).Single().Variants[0].Stock.ShouldBe(1);
    }
}
