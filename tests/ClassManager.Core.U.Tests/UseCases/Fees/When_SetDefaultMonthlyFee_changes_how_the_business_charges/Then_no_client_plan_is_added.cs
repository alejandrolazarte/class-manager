using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_SetDefaultMonthlyFee_changes_how_the_business_charges;

public sealed class Then_no_client_plan_is_added
{
    [Fact]
    public async Task Then_no_client_plan_is_added_Run()
    {
        var builder = new FeeUseCaseBuilder();
        builder.FeeSchedule.Setup(repository => repository.ListClientIdsWithoutPlanAsync(It.IsAny<CancellationToken>())).ReturnsAsync([TestIds.Next()]);

        await builder.BuildSetDefaultFee().ExecuteAsync(
            new SetDefaultMonthlyFeeCommand(null, null, BusinessBillingMode.ClassPacks), CancellationToken.None);

        builder.FeeSchedule.Verify(repository => repository.Add(It.IsAny<DefaultMonthlyFeeChange>()), Times.Once);
        builder.FeeSchedule.Verify(repository => repository.Add(It.IsAny<ClientBillingPlanChange>()), Times.Never);
    }
}
