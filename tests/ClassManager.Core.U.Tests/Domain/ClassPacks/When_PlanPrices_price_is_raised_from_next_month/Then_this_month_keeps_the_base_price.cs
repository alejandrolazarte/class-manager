using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_PlanPrices_price_is_raised_from_next_month;

public sealed class Then_this_month_keeps_the_base_price
{
    [Fact]
    public void Then_this_month_keeps_the_base_price_Run()
    {
        var plan = ClassPack.Create("Cuota mensual", null, 25000m, null, TestData.Now, renewsMonthly: true).Value!.WithId();
        var thisMonth = BillingMonth.From(TestData.Today);
        var raise = ClassPackPriceChange.Create(plan.Id, thisMonth.AddMonths(1), 28000m, TestData.Today, TestData.Now).Value!;

        var priceThisMonth = PlanPrices.PriceIn(plan, [raise], thisMonth);

        priceThisMonth.ShouldBe(25000m);
        PlanPrices.PriceIn(plan, [raise], thisMonth.AddMonths(1)).ShouldBe(28000m);
    }
}
