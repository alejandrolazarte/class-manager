using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_SetDefaultMonthlyFee_month_already_has_a_change_to_class_packs;

public sealed class Then_the_change_switches_to_monthly_fee
{
    [Fact]
    public async Task Then_the_change_switches_to_monthly_fee_Run()
    {
        var builder = new FeeUseCaseBuilder();
        builder.AddDefaultFeeChange(builder.CurrentMonth, null, BusinessBillingMode.ClassPacks);
        var existingChange = builder.DefaultFeeChanges.Last();
        builder.FeeSchedule
            .Setup(repository => repository.FindDefaultFeeChangeForUpdateAsync(existingChange.EffectiveFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingChange);

        await builder.BuildSetDefaultFee().ExecuteAsync(
            new SetDefaultMonthlyFeeCommand(15000m, builder.CurrentMonth.ToString(), BusinessBillingMode.MonthlyFee), CancellationToken.None);

        existingChange.BillingMode.ShouldBe(BusinessBillingMode.MonthlyFee);
    }
}
