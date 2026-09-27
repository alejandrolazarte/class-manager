using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_SetClientBillingPlan_to_class_packs;

public sealed class Then_the_plan_applies_from_the_current_month
{
    [Fact]
    public async Task Then_the_plan_applies_from_the_current_month_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var client = TestData.Client();
        builder.Clients.Setup(repository => repository.GetByIdAsync(client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        builder.FeeSchedule
            .Setup(repository => repository.Add(It.IsAny<ClientBillingPlanChange>()))
            .Callback<ClientBillingPlanChange>(builder.PlanChanges.Add);

        var response = await builder.BuildSetClientPlan().ExecuteAsync(
            new SetClientBillingPlanCommand(client.Id, BillingPlanKind.ClassPacks, null, null), CancellationToken.None);

        response.Value!.BillingPlan.Kind.ShouldBe(BillingPlanKind.ClassPacks);
        response.Value.BillingPlanChanges.Single().EffectiveFrom.ShouldBe(builder.CurrentMonth.ToString());
    }
}
