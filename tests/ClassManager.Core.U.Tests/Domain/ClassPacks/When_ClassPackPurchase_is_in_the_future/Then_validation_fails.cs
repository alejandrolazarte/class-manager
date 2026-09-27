using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPackPurchase_is_in_the_future;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var purchase = ClassPackPurchase.Sell(
            ClassPackTestData.ClientId, ClassPackTestData.Pack(), null, TestData.Today.AddDays(1), PaymentMethod.Cash, null, TestData.Today, TestData.Now);

        purchase.Error!.FieldName.ShouldBe(nameof(ClassPackPurchase.PurchasedOn));
    }
}
