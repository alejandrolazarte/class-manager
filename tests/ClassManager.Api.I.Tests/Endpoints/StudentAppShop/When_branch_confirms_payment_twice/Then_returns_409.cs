namespace ClassManager.Api.I.Tests.Endpoints.StudentAppShop.When_branch_confirms_payment_twice;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var classPack = await owner.CreateClassPackAsync();
        var order = await scenario.Student.PlaceStudentAppOrderAsync(StudentAppShopRequests.PackLine(classPack.Id));
        (await owner.PutOrderPaymentAsync(order.Id)).EnsureSuccessStatusCode();

        using var response = await owner.PutOrderPaymentAsync(order.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await owner.GetClassBalanceAsync(scenario.ClientId))!.AvailableClasses.ShouldBe(4);
    }
}
