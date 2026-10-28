namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPackPurchase_pack_changes_its_classes_after_the_sale;

public sealed class Then_the_sale_keeps_the_classes_it_was_sold_with
{
    [Fact]
    public void Then_the_sale_keeps_the_classes_it_was_sold_with_Run()
    {
        var pack = ClassPackTestData.Pack();
        pack.DefineLessons([ClassPackTestData.ClassGroupId], null);
        var purchase = ClassPackTestData.Sell(pack, new DateOnly(2026, 9, 1));

        pack.DefineLessons([TestIds.Next()], null);

        purchase.CoversGroupClass(ClassPackTestData.ClassGroupId).ShouldBeTrue();
    }
}
