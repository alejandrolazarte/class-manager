using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_plans_are_listed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_each_plan_has_its_student_limit(ApiFixture fixture)
{
    private const int FreeStudents = 30;
    private const int LiteStudents = 100;
    private const int ProStudents = 300;

    [Fact]
    public async Task Then_each_plan_has_its_student_limit_Run()
    {
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        var plans = await anonymousClient.ListPlansAsync();

        plans.ToDictionary(plan => plan.Code, plan => plan.Features.Single(feature => feature.Code == Features.Students).Limit)
            .ShouldBe(new Dictionary<string, int?>
            {
                [PlanCodes.Free] = FreeStudents,
                [PlanCodes.Lite] = LiteStudents,
                [PlanCodes.Pro] = ProStudents,
                [PlanCodes.Enterprise] = null,
            });
    }
}
