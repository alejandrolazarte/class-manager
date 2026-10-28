using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassBalance_only_one_pack_covers_the_class;

public sealed class Then_that_pack_pays_it_even_if_another_expires_first
{
    [Fact]
    public void Then_that_pack_pays_it_even_if_another_expires_first_Run()
    {
        var otherClassGroupId = TestIds.Next();
        var expiringFirstPack = ClassPackTestData.Pack(validityMonths: 1, name: "Aquagym");
        expiringFirstPack.DefineLessons([otherClassGroupId], null);
        var expiringFirst = ClassPackTestData.Sell(expiringFirstPack, new DateOnly(2026, 9, 1));
        var coveringPack = ClassPackTestData.Pack(name: "Natación");
        coveringPack.DefineLessons([ClassPackTestData.ClassGroupId], null);
        var covering = ClassPackTestData.Sell(coveringPack, new DateOnly(2026, 9, 1));

        var balance = ClassBalance.Calculate([expiringFirst, covering], [ClassPackTestData.Attended(new DateOnly(2026, 9, 3))], new DateOnly(2026, 9, 6));

        balance.PaidClasses.Single().Purchase.ShouldBe(covering);
    }
}
