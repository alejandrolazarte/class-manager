using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_DeleteClientBillingPlanChange_for_the_current_month;

public sealed class Then_validation_fails
{
    [Fact]
    public async Task Then_validation_fails_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var client = TestData.Client();
        builder.Clients.Setup(repository => repository.GetByIdAsync(client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(client);

        var response = await builder.BuildDeleteClientPlanChange().ExecuteAsync(
            new DeleteClientBillingPlanChangeCommand(client.Id, builder.CurrentMonth.ToString()), CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(DeleteClientBillingPlanChangeCommand.EffectiveFrom));
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
