using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPackPurchase_pack_renews_monthly;

public sealed class Then_it_is_not_sold
{
    [Fact]
    public void Then_it_is_not_sold_Run()
    {
        var plan = ClassPack.Create("Cuota mensual", null, 25000m, null, TestData.Now, renewsMonthly: true).Value!.WithId();

        var purchase = ClassPackPurchase.Sell(TestIds.Next(), plan, null, TestData.CurrencyCode, TestData.Today, null, null, TestData.Today, TestData.Now, null);

        purchase.Error!.Code.ShouldBe(ClassPackErrorCodes.RenewsMonthly);
    }
}
