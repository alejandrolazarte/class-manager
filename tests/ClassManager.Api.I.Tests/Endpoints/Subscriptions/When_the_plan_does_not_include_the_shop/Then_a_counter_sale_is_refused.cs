using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_plan_does_not_include_the_shop;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_counter_sale_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_counter_sale_is_refused_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        await fixture.ChangePlanAsync(scenario.Instructors.Business.Business.Id, PlanCodes.Lite);
        var classPack = await owner.CreateClassPackAsync();

        using var response = await owner.PostCounterSaleAsync(scenario.ClientId, [OrderRequests.PackLine(classPack.Id)]);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(FeatureErrorCodes.NotInPlan);
        problem.GetProperty(FeatureErrorCodes.FeatureDetail).GetString().ShouldBe(Features.Shop);
    }
}
