namespace ClassManager.Api.I.Tests.Endpoints.OnlineOrderPayments.When_stripe_sends_the_same_order_payment_twice;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_recorded_once(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_recorded_once_Run()
    {
        using var scenario = await fixture.SeedOnlineOrderScenarioAsync();
        var externalPaymentId = "pi_" + Guid.NewGuid().ToString("N");
        using var firstResponse = await fixture.PostCheckoutCompletedAsync(scenario, externalPaymentId);

        using var secondResponse = await fixture.PostCheckoutCompletedAsync(scenario, externalPaymentId);

        secondResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await scenario.FamilyOrderAsync()).PaidAmount.ShouldBe(scenario.Order.Balance);
    }
}
