using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_plans_are_listed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_active_plans_are_returned_in_order(ApiFixture fixture)
{
    private const int FreeTrialDays = 30;

    [Fact]
    public async Task Then_active_plans_are_returned_in_order_Run()
    {
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        var plans = await anonymousClient.ListPlansAsync();

        plans.Select(plan => plan.Code).ShouldBe(PlanCodes.All);
        plans[0].DurationInDays.ShouldBe(FreeTrialDays);
        plans.Single(plan => plan.Code == PlanCodes.Pro).Features.ShouldContain(feature => feature.Code == Features.Shop);
    }
}
