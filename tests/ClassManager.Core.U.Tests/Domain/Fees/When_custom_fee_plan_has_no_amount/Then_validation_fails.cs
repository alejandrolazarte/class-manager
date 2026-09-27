using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_custom_fee_plan_has_no_amount;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var change = ClientBillingPlanChange.Create(
            Guid.CreateVersion7(), BillingMonth.From(TestData.Today), BillingPlanKind.CustomFee, null, TestData.Today, TestData.Now);

        change.Error!.FieldName.ShouldBe(nameof(ClientBillingPlanChange.CustomFee));
    }
}
