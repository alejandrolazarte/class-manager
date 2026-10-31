using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_PlanCoverage_plan_is_for_other_classes;

public sealed class Then_the_class_is_not_covered
{
    [Fact]
    public void Then_the_class_is_not_covered_Run()
    {
        var plan = ClassPack.Create("Plan", null, 25000m, null, TestData.Now, renewsMonthly: true).Value!.WithId();
        plan.DefineLessons([TestIds.Next()], null);
        var attended = PlanCoverageTestData.Attended(day: 3);

        var notCovered = PlanCoverage.NotCoveredByPlans(
            [attended],
            [PlanCoverageTestData.Holds(plan)],
            new Dictionary<long, ClassPack> { [plan.Id] = plan });

        notCovered.ShouldBe([attended]);
    }
}
