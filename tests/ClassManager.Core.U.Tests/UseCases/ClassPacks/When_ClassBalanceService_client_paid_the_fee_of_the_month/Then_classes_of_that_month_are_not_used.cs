using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.UseCases.ClassPacks.When_ClassBalanceService_client_paid_the_fee_of_the_month;

public sealed class Then_classes_of_that_month_are_not_used
{
    [Fact]
    public async Task Then_classes_of_that_month_are_not_used_Run()
    {
        var builder = new ClassPackUseCaseBuilder();
        var clientId = Guid.CreateVersion7();
        var currentMonth = BillingMonth.From(TestData.Today);
        builder.PayPerClassFrom(clientId, currentMonth);
        builder.PayFee(clientId, currentMonth);
        builder.ClientPurchases.Add(ClassPackPurchase.Sell(
            clientId, builder.Pack, null, currentMonth.FirstDay, PaymentMethod.Cash, null, TestData.Today, TestData.Now).Value!);
        builder.Attend(clientId, currentMonth.FirstDay);

        var balances = await builder.BuildBalanceService().CalculateAsync([clientId], CancellationToken.None);

        balances[clientId].Purchases.Single().UsedClasses.ShouldBe(0);
    }
}
