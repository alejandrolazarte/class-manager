namespace ClassManager.Api.I.Tests.Endpoints.OnlineOrderPayments.When_the_brand_has_no_online_payments_add_on;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_order_cannot_be_paid_online(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_order_cannot_be_paid_online_Run()
    {
        using var scenario = await fixture.SeedOnlineOrderScenarioAsync(withAddOn: false);

        var order = await scenario.FamilyOrderAsync();

        order.CanPayOnline.ShouldBeFalse();
    }
}
