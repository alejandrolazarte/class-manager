namespace ClassManager.Api.I.Tests.Endpoints.OnlineOrderPayments.When_the_branch_charges_online;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_an_unpaid_order_can_be_paid_online(ApiFixture fixture)
{
    [Fact]
    public async Task Then_an_unpaid_order_can_be_paid_online_Run()
    {
        using var scenario = await fixture.SeedOnlineOrderScenarioAsync();

        var order = await scenario.FamilyOrderAsync();

        order.CanPayOnline.ShouldBeTrue();
    }
}
