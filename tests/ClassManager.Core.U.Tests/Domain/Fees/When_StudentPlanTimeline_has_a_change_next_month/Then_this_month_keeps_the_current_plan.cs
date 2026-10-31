using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_StudentPlanTimeline_has_a_change_next_month;

public sealed class Then_this_month_keeps_the_current_plan
{
    [Fact]
    public void Then_this_month_keeps_the_current_plan_Run()
    {
        var studentId = TestIds.Next();
        var monthlyPlan = ClassPack.Create("Cuota mensual", null, 25000m, null, TestData.Now, renewsMonthly: true).Value!.WithId();
        var thisMonth = BillingMonth.From(TestData.Today);
        var changes = new[]
        {
            StudentPlanChange.Create(studentId, thisMonth, monthlyPlan, null, TestData.Today, TestData.Now).Value!,
            StudentPlanChange.Create(studentId, thisMonth.AddMonths(1), null, null, TestData.Today, TestData.Now).Value!,
        };

        var plan = StudentPlanTimeline.ChangeIn(changes, thisMonth);

        plan!.ClassPackId.ShouldBe(monthlyPlan.Id);
        StudentPlanTimeline.ChangeIn(changes, thisMonth.AddMonths(1))!.ClassPackId.ShouldBeNull();
    }
}
