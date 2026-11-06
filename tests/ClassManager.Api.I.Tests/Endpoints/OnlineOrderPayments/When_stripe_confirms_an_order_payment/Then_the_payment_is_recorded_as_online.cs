using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.OnlineOrderPayments.When_stripe_confirms_an_order_payment;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_payment_is_recorded_as_online(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_payment_is_recorded_as_online_Run()
    {
        using var scenario = await fixture.SeedOnlineOrderScenarioAsync();

        using var response = await fixture.PostCheckoutCompletedAsync(scenario, "pi_" + Guid.NewGuid().ToString("N"));

        var order = (await scenario.Owner.ListOrdersAsync()).Single(candidate => candidate.Id == scenario.Order.Id);
        var payments = await scenario.Owner.GetFromJsonAsync<OrderResponse>($"{ApiRoutes.Orders}/{order.Id}", ApiRequests.JsonOptions);
        payments!.Payments.Single().Method.ShouldBe(PaymentMethod.Online);
    }
}
