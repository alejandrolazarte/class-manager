using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassBalance_pack_does_not_cover_the_class;

public sealed class Then_the_class_is_unpaid
{
    [Fact]
    public void Then_the_class_is_unpaid_Run()
    {
        var pack = ClassPackTestData.Pack();
        pack.DefineLessons([ClassPackTestData.ClassGroupId], null);
        var purchase = ClassPackTestData.Sell(pack, new DateOnly(2026, 9, 1));

        var balance = ClassBalance.Calculate([purchase], [ClassPackTestData.Attended(new DateOnly(2026, 9, 3), TestIds.Next())], new DateOnly(2026, 9, 6));

        balance.UnpaidClasses.ShouldBe(1);
    }
}
