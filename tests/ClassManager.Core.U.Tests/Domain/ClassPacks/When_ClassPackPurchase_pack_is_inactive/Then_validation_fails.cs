using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPackPurchase_pack_is_inactive;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var pack = ClassPackTestData.Pack();
        pack.Deactivate();

        var purchase = ClassPackPurchase.Sell(ClassPackTestData.ClientId, pack, null, TestData.Today, PaymentMethod.Cash, null, TestData.Today, TestData.Now);

        purchase.Error!.FieldName.ShouldBe(nameof(ClassPackPurchase.ClassPackId));
    }
}
