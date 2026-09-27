using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_SetDefaultMonthlyFee_without_month;

public sealed class Then_it_applies_from_the_current_month
{
    [Fact]
    public async Task Then_it_applies_from_the_current_month_Run()
    {
        var builder = new FeeUseCaseBuilder();
        DefaultMonthlyFeeChange? addedChange = null;
        builder.FeeSchedule
            .Setup(repository => repository.Add(It.IsAny<DefaultMonthlyFeeChange>()))
            .Callback<DefaultMonthlyFeeChange>(change => addedChange = change);

        await builder.BuildSetDefaultFee().ExecuteAsync(new SetDefaultMonthlyFeeCommand(15000m, null), CancellationToken.None);

        addedChange!.EffectiveFrom.ShouldBe(builder.CurrentMonth.FirstDay);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
