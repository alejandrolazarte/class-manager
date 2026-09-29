namespace ClassManager.Api.I.Tests.Endpoints.FamilyShop.When_family_cancels_the_order_of_another_family;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var otherFamily = await fixture.InviteFamilyOfAsync(scenario.Coaches, CoachScenario.OtherStudentFullName);
        var classPack = await scenario.Coaches.Business.HttpClient.CreateClassPackAsync();
        var otherOrder = await otherFamily.Family.PlaceFamilyOrderAsync(FamilyShopRequests.PackLine(classPack.Id));

        using var response = await scenario.Family.PutFamilyOrderCancellationAsync(otherOrder.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await scenario.Family.ListFamilyOrdersAsync()).ShouldBeEmpty();
    }
}
