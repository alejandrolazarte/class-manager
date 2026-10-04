using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.UseCases.ClassPacks.When_ClassBalanceService_client_attends_group_and_private_classes;

public sealed class Then_both_use_classes_from_the_pack
{
    [Fact]
    public async Task Then_both_use_classes_from_the_pack_Run()
    {
        var builder = new ClassPackUseCaseBuilder();
        var clientId = Guid.CreateVersion7();
        var currentMonth = BillingMonth.From(TestData.Today);
        builder.PayPerClassFrom(clientId, currentMonth);
        builder.ClientPurchases.Add(ClassPackPurchase.Sell(
            clientId, builder.Pack, null, currentMonth.FirstDay, PaymentMethod.Cash, null, TestData.Today, TestData.Now).Value!);
        builder.Attend(clientId, currentMonth.FirstDay.AddDays(1));
        builder.AttendPrivateLesson(clientId, currentMonth.FirstDay.AddDays(2));

        var balances = await builder.BuildBalanceService().CalculateAsync([clientId], CancellationToken.None);

        balances[clientId].Purchases.Single().UsedClasses.ShouldBe(2);
    }
}
