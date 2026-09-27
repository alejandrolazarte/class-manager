using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_default_fee_has_no_change_yet;

public sealed class Then_there_is_no_fee
{
    [Fact]
    public void Then_there_is_no_fee_Run()
    {
        var changes = new[]
        {
            DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-10").Value!, 25000m, TestData.Today, TestData.Now).Value!,
        };

        var fee = FeeTimeline.DefaultFeeIn(changes, BillingMonth.Parse("2026-09").Value!);

        fee.ShouldBeNull();
    }
}
