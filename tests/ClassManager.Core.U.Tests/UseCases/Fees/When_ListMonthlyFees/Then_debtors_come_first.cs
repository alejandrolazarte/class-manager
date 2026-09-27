using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_ListMonthlyFees;

public sealed class Then_debtors_come_first
{
    [Fact]
    public async Task Then_debtors_come_first_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var paidClientId = builder.AddEnrolledClient("Aldana Paz");
        builder.AddEnrolledClient("Zoe Ruiz");
        builder.PaidByClient[paidClientId] = FeeUseCaseBuilder.DefaultFee;

        var response = await builder.BuildList().ExecuteAsync(new ListMonthlyFeesQuery(null), CancellationToken.None);

        response.Value!.Clients.Select(client => client.ClientFullName).ShouldBe(["Zoe Ruiz", "Aldana Paz"]);
        response.Value.TotalDue.ShouldBe(FeeUseCaseBuilder.DefaultFee * 2);
        response.Value.TotalPaid.ShouldBe(FeeUseCaseBuilder.DefaultFee);
    }
}
