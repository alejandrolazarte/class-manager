using ClassManager.Core.Domain.Orders;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppShop.When_branch_hands_over_a_student_order;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_classes_are_credited(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_classes_are_credited_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var classPack = await owner.CreateClassPackAsync();
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        var order = await scenario.Student.PlaceStudentAppOrderAsync(
            StudentAppShopRequests.PackLine(classPack.Id), StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1));

        using var response = await owner.PutDeliveredAsync(order.Id);

        var deliveredOrder = (await response.Content.ReadFromJsonAsync<OrderResponse>(ApiRequests.JsonOptions))!;
        (deliveredOrder.Status, deliveredOrder.PaymentStatus).ShouldBe((OrderStatus.Delivered, OrderPaymentStatus.Unpaid));
        (await owner.GetClassBalanceAsync(scenario.ClientId))!.AvailableClasses.ShouldBe(4);
    }
}
