using ClassManager.Core.Domain.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.OnlineOrderPayments.When_stripe_confirms_an_order_payment;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_order_is_paid_online(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_order_is_paid_online_Run()
    {
        using var scenario = await fixture.SeedOnlineOrderScenarioAsync();

        using var response = await fixture.PostCheckoutCompletedAsync(scenario, "pi_" + Guid.NewGuid().ToString("N"));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var order = await scenario.FamilyOrderAsync();
        (order.PaymentStatus, order.CanPayOnline).ShouldBe((OrderPaymentStatus.Paid, false));
    }
}
