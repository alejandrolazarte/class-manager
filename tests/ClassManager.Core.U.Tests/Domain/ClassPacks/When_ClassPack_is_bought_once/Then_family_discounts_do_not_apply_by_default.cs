using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_is_bought_once;

public sealed class Then_family_discounts_do_not_apply_by_default
{
    [Fact]
    public void Then_family_discounts_do_not_apply_by_default_Run()
    {
        var pack = ClassPack.Create("4 clases", 4, 10000m, 1, TestData.Now);

        pack.Value!.AppliesFamilyDiscount.ShouldBeFalse();
    }
}
