using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_plan_does_not_include_the_shop;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_pack_ordered_from_the_app_is_collected_and_handed_over(ApiFixture fixture)
{
    private const int PackClassCount = 4;

    [Fact]
    public async Task Then_a_pack_ordered_from_the_app_is_collected_and_handed_over_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        await fixture.ChangePlanAsync(scenario.Instructors.Business.Business.Id, PlanCodes.Lite);
        var classPack = await owner.CreateClassPackAsync(classCount: PackClassCount);
        var order = await scenario.Student.PlaceStudentAppOrderAsync(StudentAppShopRequests.PackLine(classPack.Id));

        using var response = await owner.PostOrderPaymentAsync(order.Id, isDelivered: true);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await owner.GetClassBalanceAsync(scenario.ClientId))!.AvailableClasses.ShouldBe(PackClassCount);
    }
}
