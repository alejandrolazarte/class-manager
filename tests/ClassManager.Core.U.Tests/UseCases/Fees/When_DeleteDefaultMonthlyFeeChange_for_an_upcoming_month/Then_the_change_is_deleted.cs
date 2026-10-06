using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_DeleteDefaultMonthlyFeeChange_for_an_upcoming_month;

public sealed class Then_the_change_is_deleted
{
    [Fact]
    public async Task Then_the_change_is_deleted_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var upcomingMonth = builder.CurrentMonth.AddMonths(1);
        builder.AddDefaultFeeChange(upcomingMonth, 15000m);
        var upcomingChange = builder.DefaultFeeChanges.Last();
        builder.FeeSchedule
            .Setup(repository => repository.FindDefaultFeeChangeForUpdateAsync(upcomingMonth.FirstDay, It.IsAny<CancellationToken>()))
            .ReturnsAsync(upcomingChange);

        await builder.BuildDeleteDefaultFeeChange().ExecuteAsync(
            new DeleteDefaultMonthlyFeeChangeCommand(upcomingMonth.ToString()), CancellationToken.None);

        upcomingChange.DeletedOn.ShouldBe(TestData.Now);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
