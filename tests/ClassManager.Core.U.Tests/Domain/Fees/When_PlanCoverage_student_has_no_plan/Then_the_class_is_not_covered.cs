using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_PlanCoverage_student_has_no_plan;

public sealed class Then_the_class_is_not_covered
{
    [Fact]
    public void Then_the_class_is_not_covered_Run()
    {
        var attended = PlanCoverageTestData.Attended(day: 3);

        var notCovered = PlanCoverage.NotCoveredByPlans([attended], [], new Dictionary<long, ClassPack>());

        notCovered.ShouldBe([attended]);
    }
}
