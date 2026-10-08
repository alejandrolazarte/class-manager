using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_ListMonthlyFees_client_switched_to_class_packs_after_paying;

public sealed class Then_the_payment_counts_as_collected
{
    [Fact]
    public async Task Then_the_payment_counts_as_collected_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var monthlyClientId = builder.AddEnrolledClient("Carla Sosa");
        builder.PaidByClient[monthlyClientId] = 4000m;
        var classPackClientId = builder.AddEnrolledClient("Ana Pérez");
        builder.SetPlan(classPackClientId, BillingPlanKind.ClassPacks, effectiveFrom: builder.CurrentMonth);
        builder.PaidByClient[classPackClientId] = FeeUseCaseBuilder.DefaultFee;
        builder.ClassBalancesByClient[classPackClientId] = new ClassBalance([], []);

        var response = await builder.BuildList().ExecuteAsync(new ListMonthlyFeesQuery(null), CancellationToken.None);

        response.Value!.TotalPaid.ShouldBe(4000m + FeeUseCaseBuilder.DefaultFee);
    }
}
