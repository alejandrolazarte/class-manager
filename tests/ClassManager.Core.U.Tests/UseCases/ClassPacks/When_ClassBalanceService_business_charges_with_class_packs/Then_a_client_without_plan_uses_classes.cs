using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.UseCases.ClassPacks.When_ClassBalanceService_business_charges_with_class_packs;

public sealed class Then_a_client_without_plan_uses_classes
{
    [Fact]
    public async Task Then_a_client_without_plan_uses_classes_Run()
    {
        var builder = new ClassPackUseCaseBuilder();
        var clientId = TestIds.Next();
        var currentMonth = BillingMonth.From(TestData.Today);
        builder.ChargeBusinessWithClassPacksFrom(currentMonth);
        builder.Attend(clientId, currentMonth.FirstDay.AddDays(-7));
        builder.Attend(clientId, currentMonth.FirstDay.AddDays(1));

        var balances = await builder.BuildBalanceService().CalculateAsync([clientId], CancellationToken.None);

        balances[clientId].UnpaidClasses.ShouldBe(1);
    }
}
