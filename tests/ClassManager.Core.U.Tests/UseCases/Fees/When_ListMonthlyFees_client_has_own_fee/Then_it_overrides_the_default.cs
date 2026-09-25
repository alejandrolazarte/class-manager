using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_ListMonthlyFees_client_has_own_fee;

public sealed class Then_it_overrides_the_default
{
    [Fact]
    public async Task Then_it_overrides_the_default_Run()
    {
        var builder = new FeeUseCaseBuilder();
        builder.AddEnrolledClient("Ana Pérez", clientFee: 10000m);

        var response = await builder.BuildList().ExecuteAsync(new ListMonthlyFeesQuery(null), CancellationToken.None);

        response.Value!.Clients.Single().Fee.ShouldBe(10000m);
    }
}
