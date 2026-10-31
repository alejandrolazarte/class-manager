using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees;

internal static class PlanCoverageTestData
{
    public static readonly long StudentId = TestIds.Next();
    public static readonly long ClassGroupId = TestIds.Next();
    public static readonly BillingMonth ThisMonth = BillingMonth.From(TestData.Today);

    public static ClassPack MonthlyPlan(int? classCount = null)
    {
        var plan = ClassPack.Create("Plan", classCount, 25000m, null, TestData.Now, renewsMonthly: true).Value!.WithId();
        plan.DefineLessons([], null, coversAnyGroupClass: true);
        return plan;
    }

    public static StudentPlanChange Holds(ClassPack plan) =>
        StudentPlanChange.Create(StudentId, ThisMonth, plan, null, TestData.Today, TestData.Now).Value!;

    public static AttendedClass Attended(int day, long? classGroupId = null) =>
        new(ThisMonth.FirstDay.AddDays(day - 1), TestData.StudentFullName, "Clase", StudentId: StudentId, ClassGroupId: classGroupId ?? ClassGroupId);
}
