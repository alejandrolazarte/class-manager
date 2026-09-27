using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_default_fee_is_raised_in_march;

public sealed class Then_january_keeps_the_previous_fee
{
    [Fact]
    public void Then_january_keeps_the_previous_fee_Run()
    {
        var today = new DateOnly(2026, 3, 10);
        var changes = new[]
        {
            DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-01").Value!, 25000m, today, TestData.Now).Value!,
            DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-03").Value!, 30000m, today, TestData.Now).Value!,
        };

        var januaryFee = FeeTimeline.DefaultFeeIn(changes, BillingMonth.Parse("2026-01").Value!);

        januaryFee.ShouldBe(25000m);
        FeeTimeline.DefaultFeeIn(changes, BillingMonth.Parse("2026-04").Value!).ShouldBe(30000m);
    }
}
