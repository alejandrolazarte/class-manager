using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppShop.When_student_cancels_a_counter_sale_not_paid_yet;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var product = await owner.CreateProductAsync(StockMode.Unlimited);
        var order = await owner.SellAtCounterAsync(
            scenario.ClientId, [OrderRequests.ProductLine(product.Variants[0].Id, 1)], isDelivered: false, paidAmount: 0m);

        using var response = await scenario.Student.PutStudentAppOrderCancellationAsync(order.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await scenario.Student.ListStudentAppOrdersAsync()).Single().Status.ShouldBe(OrderStatus.Pending);
    }
}
