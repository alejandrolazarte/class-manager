using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_ListMonthlyFees_client_paid_part;

public sealed class Then_status_is_partial_with_balance
{
    [Fact]
    public async Task Then_status_is_partial_with_balance_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var clientId = builder.AddEnrolledClient("Ana Pérez", null, "Tomás Pérez", "Lucía Pérez");
        builder.PaidByClient[clientId] = 5000m;

        var response = await builder.BuildList().ExecuteAsync(new ListMonthlyFeesQuery(null), CancellationToken.None);

        var client = response.Value!.Clients.Single();
        client.Status.ShouldBe(FeeStatus.Partial);
        client.Balance.ShouldBe(FeeUseCaseBuilder.DefaultFee - 5000m);
        client.StudentNames.ShouldBe(["Lucía Pérez", "Tomás Pérez"]);
    }
}
