using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_DeleteDefaultMonthlyFeeChange_for_the_current_month;

public sealed class Then_validation_fails
{
    [Fact]
    public async Task Then_validation_fails_Run()
    {
        var builder = new FeeUseCaseBuilder();

        var response = await builder.BuildDeleteDefaultFeeChange().ExecuteAsync(
            new DeleteDefaultMonthlyFeeChangeCommand(builder.CurrentMonth.ToString()), CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(DeleteDefaultMonthlyFeeChangeCommand.EffectiveFrom));
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
