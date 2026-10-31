using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_StudentPlanChange_plan_is_a_pack_bought_once;

public sealed class Then_validation_fails_on_the_plan
{
    [Fact]
    public void Then_validation_fails_on_the_plan_Run()
    {
        var pack = ClassPack.Create("4 clases", 4, 10000m, 1, TestData.Now).Value!.WithId();

        var change = StudentPlanChange.Create(TestIds.Next(), BillingMonth.From(TestData.Today), pack, null, TestData.Today, TestData.Now);

        change.Error!.FieldName.ShouldBe(nameof(StudentPlanChange.ClassPackId));
    }
}
