using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassBalance_pack_expired_with_classes_left;

public sealed class Then_they_are_lost
{
    [Fact]
    public void Then_they_are_lost_Run()
    {
        var purchase = ClassPackTestData.Purchase(new DateOnly(2026, 8, 1), classCount: 4, validityMonths: 1);

        var balance = ClassBalance.Calculate([purchase], [ClassPackTestData.Attended(new DateOnly(2026, 8, 8))], new DateOnly(2026, 9, 10));

        balance.AvailableClasses.ShouldBe(0);
        var usage = balance.Purchases.Single();
        usage.Status.ShouldBe(ClassPackPurchaseStatus.Expired);
        usage.RemainingClasses.ShouldBe(3);
    }
}
