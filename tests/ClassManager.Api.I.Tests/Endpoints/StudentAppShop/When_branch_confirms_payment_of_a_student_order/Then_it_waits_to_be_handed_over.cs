using ClassManager.Core.Domain.Orders;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppShop.When_branch_confirms_payment_of_a_student_order;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_waits_to_be_handed_over(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_waits_to_be_handed_over_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var classPack = await owner.CreateClassPackAsync();
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        var order = await scenario.Student.PlaceStudentAppOrderAsync(
            StudentAppShopRequests.PackLine(classPack.Id), StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1));
        (await owner.ListOrdersAsync()).Single().Id.ShouldBe(order.Id);

        using var response = await owner.PostOrderPaymentAsync(order.Id);

        var paidOrder = (await response.Content.ReadFromJsonAsync<OrderResponse>(ApiRequests.JsonOptions))!;
        (paidOrder.PaymentStatus, paidOrder.AwaitsPickup).ShouldBe((OrderPaymentStatus.Paid, true));
        (await owner.GetClassBalanceAsync(scenario.ClientId))!.AvailableClasses.ShouldBe(0);
        (await owner.ListProductsAsync()).Single().Variants[0].Stock.ShouldBe(1);
    }
}
