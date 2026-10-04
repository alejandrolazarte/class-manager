using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppShop.When_student_orders_the_last_unit;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_reserved(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_reserved_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 1);

        var order = await scenario.Student.PlaceStudentAppOrderAsync(StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1));

        order.Status.ShouldBe(OrderStatus.Requested);
        (await scenario.Student.GetStudentAppShopAsync()).Products.Single().Variants[0].Availability.ShouldBe(StockAvailability.SoldOut);
        (await owner.ListProductsAsync()).Single().Variants[0].Stock.ShouldBe(0);
    }
}
