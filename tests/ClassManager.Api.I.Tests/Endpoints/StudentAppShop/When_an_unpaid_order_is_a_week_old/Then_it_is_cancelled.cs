using ClassManager.Api.Orders;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppShop.When_an_unpaid_order_is_a_week_old;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_cancelled(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_cancelled_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var productResponse = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        var oldOrder = await SeedRequestedOrderAsync(scenario, productResponse.Id, BusinessApiFactory.Now - Order.RequestLifetime);
        var recentOrder = await scenario.Student.PlaceStudentAppOrderAsync(StudentAppShopRequests.ProductLine(productResponse.Variants[0].Id, 1));

        await fixture.ApiFactory.Services.GetRequiredService<IExpiredOrderCancellationService>().CancelExpiredOrdersAsync(CancellationToken.None);

        var orders = await scenario.Student.ListStudentAppOrdersAsync();
        orders.Single(order => order.Id == oldOrder).Status.ShouldBe(OrderStatus.Cancelled);
        orders.Single(order => order.Id == recentOrder.Id).Status.ShouldBe(OrderStatus.Requested);
        (await owner.ListProductsAsync()).Single().Variants[0].Stock.ShouldBe(1);
    }

    private async Task<Guid> SeedRequestedOrderAsync(StudentAppScenario scenario, Guid productId, DateTimeOffset createdAt)
    {
        await using var context = fixture.CreateDbContext(scenario.Instructors.Business.Business.Id);
        var product = await context.Products.Include(candidate => candidate.Variants).SingleAsync(candidate => candidate.Id == productId);
        var variant = product.Variants[0];
        var order = Order.Request(scenario.ClientId, [OrderLine.ForProduct(product, variant, 1, null).Value!], null, createdAt).Value!;
        context.Orders.Add(order);
        context.StockMovements.Add(StockMovement.Reserve(variant.Id, 1, order.Id, null, createdAt));
        await context.SaveChangesAsync();
        return order.Id;
    }
}
