using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_BillingMonth_is_parsed;

public sealed class Then_first_and_last_day_are_known
{
    [Fact]
    public void Then_first_and_last_day_are_known_Run()
    {
        var month = BillingMonth.Parse("2026-02").Value!;

        month.FirstDay.ShouldBe(new DateOnly(2026, 2, 1));
        month.LastDay.ShouldBe(new DateOnly(2026, 2, 28));
        month.ToString().ShouldBe("2026-02");
    }
}
