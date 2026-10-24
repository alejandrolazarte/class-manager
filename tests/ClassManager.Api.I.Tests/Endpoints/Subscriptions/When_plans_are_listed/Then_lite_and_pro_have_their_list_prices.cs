using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_plans_are_listed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_lite_and_pro_have_their_list_prices(ApiFixture fixture)
{
    private const decimal LitePrice = 19m;
    private const decimal ProPrice = 39m;

    [Fact]
    public async Task Then_lite_and_pro_have_their_list_prices_Run()
    {
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        var plans = await anonymousClient.ListPlansAsync();

        var listPriceByPlanCode = plans.ToDictionary(plan => plan.Code, plan => plan.ListPrice);
        listPriceByPlanCode[PlanCodes.Lite].ShouldBe(LitePrice);
        listPriceByPlanCode[PlanCodes.Pro].ShouldBe(ProPrice);
    }
}
