using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_PlanCoverage_plan_has_libre_classes;

public sealed class Then_every_class_is_covered
{
    [Fact]
    public void Then_every_class_is_covered_Run()
    {
        var plan = PlanCoverageTestData.MonthlyPlan();

        var notCovered = PlanCoverage.NotCoveredByPlans(
            [.. Enumerable.Range(1, 12).Select(day => PlanCoverageTestData.Attended(day))],
            [PlanCoverageTestData.Holds(plan)],
            new Dictionary<long, Core.Domain.ClassPacks.ClassPack> { [plan.Id] = plan });

        notCovered.ShouldBeEmpty();
    }
}
