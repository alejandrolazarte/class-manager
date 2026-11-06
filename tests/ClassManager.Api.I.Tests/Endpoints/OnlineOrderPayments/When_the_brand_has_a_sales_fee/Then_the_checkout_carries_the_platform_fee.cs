namespace ClassManager.Api.I.Tests.Endpoints.OnlineOrderPayments.When_the_brand_has_a_sales_fee;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_checkout_carries_the_platform_fee(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_checkout_carries_the_platform_fee_Run()
    {
        using var scenario = await fixture.SeedOnlineOrderScenarioAsync();
        await fixture.GiveSalesFeeAsync(scenario.OrganizationId, 5m);

        await scenario.Family.StartOrderCheckoutAsync(scenario.Order.Id);

        var checkout = fixture.ApiFactory.OnlinePaymentGateway.CheckoutsFor(scenario.Order.Id).Single();
        checkout.PlatformFee.ShouldBe(decimal.Floor(scenario.Order.Balance * 5m) / 100m);
    }
}
