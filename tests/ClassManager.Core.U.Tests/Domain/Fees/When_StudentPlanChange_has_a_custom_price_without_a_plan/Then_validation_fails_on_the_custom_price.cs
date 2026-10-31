using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_StudentPlanChange_has_a_custom_price_without_a_plan;

public sealed class Then_validation_fails_on_the_custom_price
{
    [Fact]
    public void Then_validation_fails_on_the_custom_price_Run()
    {
        var change = StudentPlanChange.Create(TestIds.Next(), BillingMonth.From(TestData.Today), null, 20000m, TestData.Today, TestData.Now);

        change.Error!.FieldName.ShouldBe(nameof(StudentPlanChange.CustomPrice));
    }
}
