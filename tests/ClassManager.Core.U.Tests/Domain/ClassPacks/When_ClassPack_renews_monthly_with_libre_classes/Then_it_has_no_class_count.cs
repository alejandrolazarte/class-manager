using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_renews_monthly_with_libre_classes;

public sealed class Then_it_has_no_class_count
{
    [Fact]
    public void Then_it_has_no_class_count_Run()
    {
        var plan = ClassPack.Create("Cuota mensual", null, 25000m, null, TestData.Now, renewsMonthly: true);

        plan.Value!.ClassCount.ShouldBeNull();
        plan.Value.RenewsMonthly.ShouldBeTrue();
    }
}
