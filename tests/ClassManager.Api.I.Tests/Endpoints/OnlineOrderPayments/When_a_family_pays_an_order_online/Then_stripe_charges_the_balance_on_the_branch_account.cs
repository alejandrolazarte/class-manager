namespace ClassManager.Api.I.Tests.Endpoints.OnlineOrderPayments.When_a_family_pays_an_order_online;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_stripe_charges_the_balance_on_the_branch_account(ApiFixture fixture)
{
    [Fact]
    public async Task Then_stripe_charges_the_balance_on_the_branch_account_Run()
    {
        using var scenario = await fixture.SeedOnlineOrderScenarioAsync();

        var checkoutUrl = await scenario.Family.StartOrderCheckoutAsync(scenario.Order.Id);

        checkoutUrl.ShouldBe(FakeOnlinePaymentGateway.CheckoutUrlOf(scenario.Order.Id));
        var checkout = fixture.ApiFactory.OnlinePaymentGateway.CheckoutsFor(scenario.Order.Id).Single();
        (checkout.ExternalAccountId, checkout.Amount, checkout.PlatformFee).ShouldBe((scenario.ExternalAccountId, scenario.Order.Balance, 0m));
    }
}
