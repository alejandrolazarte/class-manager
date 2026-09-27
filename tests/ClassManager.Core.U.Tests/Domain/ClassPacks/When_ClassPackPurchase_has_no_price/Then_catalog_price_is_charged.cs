using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPackPurchase_has_no_price;

public sealed class Then_catalog_price_is_charged
{
    [Fact]
    public void Then_catalog_price_is_charged_Run()
    {
        var pack = ClassPackTestData.Pack(classCount: 8, price: 160m, validityMonths: 2);

        var purchase = ClassPackPurchase.Sell(ClassPackTestData.ClientId, pack, null, TestData.Today, PaymentMethod.Cash, null, TestData.Today, TestData.Now);

        purchase.Value!.Price.ShouldBe(160m);
    }
}
