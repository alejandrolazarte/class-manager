using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_SetClientBillingPlan_from_a_past_month;

public sealed class Then_validation_fails
{
    [Fact]
    public async Task Then_validation_fails_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var client = TestData.Client();
        builder.Clients.Setup(repository => repository.GetByIdAsync(client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        var previousMonth = builder.CurrentMonth.AddMonths(-1).ToString();

        var response = await builder.BuildSetClientPlan().ExecuteAsync(
            new SetClientBillingPlanCommand(client.Id, BillingPlanKind.CustomFee, 30m, previousMonth), CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(SetClientBillingPlanCommand.EffectiveFrom));
        builder.FeeSchedule.Verify(repository => repository.Add(It.IsAny<ClientBillingPlanChange>()), Times.Never);
    }
}
