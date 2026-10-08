using ClassManager.Core.Domain.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppShop.When_student_cancels_its_order;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_stock_is_released(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_stock_is_released_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 3);
        var order = await scenario.Student.PlaceStudentAppOrderAsync(StudentAppShopRequests.ProductLine(product.Variants[0].Id, 2));

        using var response = await scenario.Student.PutStudentAppOrderCancellationAsync(order.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await owner.ListProductsAsync()).Single().Variants[0].Stock.ShouldBe(3);
        (await scenario.Student.ListStudentAppOrdersAsync()).Single().Status.ShouldBe(OrderStatus.Cancelled);
    }
}
