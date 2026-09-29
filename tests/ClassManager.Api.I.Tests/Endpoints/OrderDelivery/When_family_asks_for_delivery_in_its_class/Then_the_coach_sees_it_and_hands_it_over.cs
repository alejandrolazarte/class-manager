using ClassManager.Core.Domain.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.OrderDelivery.When_family_asks_for_delivery_in_its_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_coach_sees_it_and_hands_it_over(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_coach_sees_it_and_hands_it_over_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var classGroupId = scenario.Coaches.CoachClassGroup.Id;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        (await scenario.Family.GetFamilyShopAsync()).DeliveryClasses.Single().ClassGroupId.ShouldBe(classGroupId);
        using var placed = await scenario.Family.PostFamilyOrderForClassAsync(
            classGroupId, FamilyShopRequests.ProductLine(product.Variants[0].Id, 1));
        placed.StatusCode.ShouldBe(HttpStatusCode.Created);
        var orderId = (await scenario.Family.ListFamilyOrdersAsync()).Single().Id;
        (await owner.PutOrderPaymentAsync(orderId)).EnsureSuccessStatusCode();
        (await scenario.Coaches.Coach.ListClassDeliveriesAsync(classGroupId)).Single().OrderId.ShouldBe(orderId);

        using var delivered = await scenario.Coaches.Coach.PutClassDeliveryAsync(classGroupId, orderId);

        delivered.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await scenario.Family.ListFamilyOrdersAsync()).Single().Status.ShouldBe(OrderStatus.Delivered);
        (await scenario.Coaches.Coach.ListClassDeliveriesAsync(classGroupId)).ShouldBeEmpty();
    }
}
