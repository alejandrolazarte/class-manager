using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_SetDefaultMonthlyFee_month_already_has_a_change;

public sealed class Then_it_is_replaced
{
    [Fact]
    public async Task Then_it_is_replaced_Run()
    {
        var builder = new FeeUseCaseBuilder();
        builder.AddDefaultFeeChange(builder.CurrentMonth, 13000m);
        var existingChange = builder.DefaultFeeChanges.Last();
        builder.FeeSchedule
            .Setup(repository => repository.FindDefaultFeeChangeForUpdateAsync(existingChange.EffectiveFrom, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingChange);

        var response = await builder.BuildSetDefaultFee().ExecuteAsync(
            new SetDefaultMonthlyFeeCommand(15000m, builder.CurrentMonth.ToString()), CancellationToken.None);

        existingChange.Amount.ShouldBe(15000m);
        response.Value!.DefaultMonthlyFee.ShouldBe(15000m);
        builder.FeeSchedule.Verify(repository => repository.Add(It.IsAny<DefaultMonthlyFeeChange>()), Times.Never);
    }
}
