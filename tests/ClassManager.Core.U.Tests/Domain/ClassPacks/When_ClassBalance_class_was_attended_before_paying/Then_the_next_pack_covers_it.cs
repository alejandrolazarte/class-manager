using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassBalance_class_was_attended_before_paying;

public sealed class Then_the_next_pack_covers_it
{
    [Fact]
    public void Then_the_next_pack_covers_it_Run()
    {
        var purchase = ClassPackTestData.Purchase(new DateOnly(2026, 9, 10), classCount: 4, validityMonths: 1);

        var balance = ClassBalance.Calculate([purchase], [ClassPackTestData.Attended(new DateOnly(2026, 9, 5))], new DateOnly(2026, 9, 11));

        balance.UnpaidClasses.ShouldBe(0);
        balance.AvailableClasses.ShouldBe(3);
    }
}
