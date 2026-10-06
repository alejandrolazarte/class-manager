using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_DeleteClientBillingPlanChange_month_has_no_change;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var client = TestData.Client();
        builder.Clients.Setup(repository => repository.GetByIdAsync(client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(client);

        var response = await builder.BuildDeleteClientPlanChange().ExecuteAsync(
            new DeleteClientBillingPlanChangeCommand(client.Id, builder.CurrentMonth.AddMonths(1).ToString()), CancellationToken.None);

        response.Error!.Code.ShouldBe(FeeErrorCodes.FeeChangeNotFound);
    }
}
