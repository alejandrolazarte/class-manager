using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_plans_are_listed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_each_plan_has_its_branch_limit(ApiFixture fixture)
{
    private const int SingleBranch = 1;
    private const int EnterpriseBranches = 3;

    [Fact]
    public async Task Then_each_plan_has_its_branch_limit_Run()
    {
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        var plans = await anonymousClient.ListPlansAsync();

        plans.ToDictionary(plan => plan.Code, plan => plan.Features.Single(feature => feature.Code == Features.Branches).Limit)
            .ShouldBe(new Dictionary<string, int?>
            {
                [PlanCodes.Free] = SingleBranch,
                [PlanCodes.Lite] = SingleBranch,
                [PlanCodes.Pro] = SingleBranch,
                [PlanCodes.Enterprise] = EnterpriseBranches,
            });
    }
}
