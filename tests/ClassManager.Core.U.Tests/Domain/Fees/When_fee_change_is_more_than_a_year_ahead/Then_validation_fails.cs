using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_fee_change_is_more_than_a_year_ahead;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var tooFarAhead = BillingMonth.From(TestData.Today).AddMonths(13);

        var change = DefaultMonthlyFeeChange.Create(tooFarAhead, 25000m, TestData.Today, TestData.Now);

        change.Error!.FieldName.ShouldBe(nameof(DefaultMonthlyFeeChange.EffectiveFrom));
    }
}
