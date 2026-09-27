using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassBalance_has_two_valid_packs;

public sealed class Then_the_one_expiring_first_is_used
{
    [Fact]
    public void Then_the_one_expiring_first_is_used_Run()
    {
        var withoutExpiry = ClassPackTestData.Purchase(new DateOnly(2026, 9, 1), classCount: 4);
        var expiringSoon = ClassPackTestData.Purchase(new DateOnly(2026, 9, 2), classCount: 4, validityMonths: 1);

        var balance = ClassBalance.Calculate([withoutExpiry, expiringSoon], [ClassPackTestData.Attended(new DateOnly(2026, 9, 5))], new DateOnly(2026, 9, 6));

        balance.Purchases.Single(usage => usage.Purchase == expiringSoon).UsedClasses.ShouldBe(1);
        balance.Purchases.Single(usage => usage.Purchase == withoutExpiry).UsedClasses.ShouldBe(0);
    }
}
