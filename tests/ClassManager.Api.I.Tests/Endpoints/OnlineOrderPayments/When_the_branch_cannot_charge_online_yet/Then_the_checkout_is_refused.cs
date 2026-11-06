namespace ClassManager.Api.I.Tests.Endpoints.OnlineOrderPayments.When_the_branch_cannot_charge_online_yet;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_checkout_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_checkout_is_refused_Run()
    {
        using var scenario = await fixture.SeedOnlineOrderScenarioAsync(chargesEnabled: false);

        using var response = await scenario.Family.PostOrderCheckoutAsync(scenario.Order.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        fixture.ApiFactory.OnlinePaymentGateway.CheckoutsFor(scenario.Order.Id).ShouldBeEmpty();
    }
}
