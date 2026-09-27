using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_ListMonthlyFees_default_fee_is_raised_next_month;

public sealed class Then_the_month_keeps_its_fee
{
    [Fact]
    public async Task Then_the_month_keeps_its_fee_Run()
    {
        var builder = new FeeUseCaseBuilder();
        builder.AddDefaultFeeChange(builder.CurrentMonth.AddMonths(1), 15000m);
        builder.AddEnrolledClient("Ana Pérez");

        var response = await builder.BuildList().ExecuteAsync(new ListMonthlyFeesQuery(null), CancellationToken.None);

        response.Value!.Clients.Single().Fee.ShouldBe(FeeUseCaseBuilder.DefaultFee);
    }
}
