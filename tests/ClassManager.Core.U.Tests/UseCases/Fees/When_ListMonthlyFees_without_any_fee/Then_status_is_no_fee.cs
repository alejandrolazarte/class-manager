using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_ListMonthlyFees_without_any_fee;

public sealed class Then_status_is_no_fee
{
    [Fact]
    public async Task Then_status_is_no_fee_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var clientId = builder.AddEnrolledClient("Ana Pérez");
        builder.SetMonthlyPrice(clientId, 0m);

        var response = await builder.BuildList().ExecuteAsync(new ListMonthlyFeesQuery(null), CancellationToken.None);

        response.Value!.Clients.Single().Status.ShouldBe(FeeStatus.NoFee);
    }
}
