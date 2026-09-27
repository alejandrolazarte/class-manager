using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassBalance_class_is_after_the_pack_expired;

public sealed class Then_it_is_unpaid
{
    [Fact]
    public void Then_it_is_unpaid_Run()
    {
        var purchase = ClassPackTestData.Purchase(new DateOnly(2026, 8, 1), classCount: 4, validityMonths: 1);

        var balance = ClassBalance.Calculate([purchase], [ClassPackTestData.Attended(new DateOnly(2026, 9, 5))], new DateOnly(2026, 9, 6));

        balance.UnpaidClasses.ShouldBe(1);
    }
}
