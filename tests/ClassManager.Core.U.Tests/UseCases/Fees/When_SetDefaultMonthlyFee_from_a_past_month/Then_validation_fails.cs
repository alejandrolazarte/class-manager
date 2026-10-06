using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_SetDefaultMonthlyFee_from_a_past_month;

public sealed class Then_validation_fails
{
    [Fact]
    public async Task Then_validation_fails_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var previousMonth = builder.CurrentMonth.AddMonths(-1).ToString();

        var response = await builder.BuildSetDefaultFee().ExecuteAsync(
            new SetDefaultMonthlyFeeCommand(15000m, previousMonth), CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(SetDefaultMonthlyFeeCommand.EffectiveFrom));
        builder.FeeSchedule.Verify(repository => repository.Add(It.IsAny<DefaultMonthlyFeeChange>()), Times.Never);
    }
}
