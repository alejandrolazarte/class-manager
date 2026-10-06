using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_DeleteClientBillingPlanChange_for_an_upcoming_month;

public sealed class Then_the_change_is_deleted
{
    [Fact]
    public async Task Then_the_change_is_deleted_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var client = TestData.Client();
        var upcomingMonth = builder.CurrentMonth.AddMonths(2);
        builder.Clients.Setup(repository => repository.GetByIdAsync(client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        builder.SetPlan(client.Id, BillingPlanKind.CustomFee, 30m, upcomingMonth);
        var upcomingChange = builder.PlanChanges.Single();
        builder.FeeSchedule
            .Setup(repository => repository.FindClientPlanChangeForUpdateAsync(client.Id, upcomingMonth.FirstDay, It.IsAny<CancellationToken>()))
            .ReturnsAsync(upcomingChange);

        await builder.BuildDeleteClientPlanChange().ExecuteAsync(
            new DeleteClientBillingPlanChangeCommand(client.Id, upcomingMonth.ToString()), CancellationToken.None);

        upcomingChange.DeletedOn.ShouldBe(TestData.Now);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
