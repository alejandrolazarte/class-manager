using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassBalance_pack_covers_any_group_class;

public sealed class Then_it_pays_a_class_it_does_not_list
{
    [Fact]
    public void Then_it_pays_a_class_it_does_not_list_Run()
    {
        var purchase = ClassPackTestData.Purchase(new DateOnly(2026, 9, 1));

        var balance = ClassBalance.Calculate([purchase], [ClassPackTestData.Attended(new DateOnly(2026, 9, 3), TestIds.Next())], new DateOnly(2026, 9, 6));

        balance.Purchases.Single().UsedClasses.ShouldBe(1);
    }
}
