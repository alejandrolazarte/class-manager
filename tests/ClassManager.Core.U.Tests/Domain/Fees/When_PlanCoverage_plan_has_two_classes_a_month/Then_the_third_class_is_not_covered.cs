using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_PlanCoverage_plan_has_two_classes_a_month;

public sealed class Then_the_third_class_is_not_covered
{
    [Fact]
    public void Then_the_third_class_is_not_covered_Run()
    {
        var plan = PlanCoverageTestData.MonthlyPlan(classCount: 2);
        var third = PlanCoverageTestData.Attended(day: 15);

        var notCovered = PlanCoverage.NotCoveredByPlans(
            [PlanCoverageTestData.Attended(day: 1), third, PlanCoverageTestData.Attended(day: 8)],
            [PlanCoverageTestData.Holds(plan)],
            new Dictionary<long, Core.Domain.ClassPacks.ClassPack> { [plan.Id] = plan });

        notCovered.ShouldBe([third]);
    }
}
