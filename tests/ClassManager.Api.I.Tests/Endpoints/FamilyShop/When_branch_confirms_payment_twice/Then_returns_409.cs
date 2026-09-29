namespace ClassManager.Api.I.Tests.Endpoints.FamilyShop.When_branch_confirms_payment_twice;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var classPack = await owner.CreateClassPackAsync();
        var order = await scenario.Family.PlaceFamilyOrderAsync(FamilyShopRequests.PackLine(classPack.Id));
        (await owner.PutOrderPaymentAsync(order.Id)).EnsureSuccessStatusCode();

        using var response = await owner.PutOrderPaymentAsync(order.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await owner.GetClassBalanceAsync(scenario.FamilyId))!.AvailableClasses.ShouldBe(4);
    }
}
