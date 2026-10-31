using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.Domain.Fees;

public static class PlanCoverage
{
    public static IReadOnlyList<AttendedClass> NotCoveredByPlans(
        IEnumerable<AttendedClass> attendedClasses,
        IReadOnlyCollection<StudentPlanChange> planChanges,
        IReadOnlyDictionary<long, ClassPack> plansById)
    {
        var changesByStudent = planChanges.ToLookup(change => change.StudentId);
        var usedClasses = new Dictionary<(long StudentId, BillingMonth Month), int>();
        var notCovered = new List<AttendedClass>();
        foreach (var attendedClass in attendedClasses.OrderBy(attended => attended.Date))
        {
            var month = BillingMonth.From(attendedClass.Date);
            var plan = attendedClass.StudentId is { } studentId
                && StudentPlanTimeline.ChangeIn(changesByStudent[studentId], month)?.ClassPackId is { } planId
                && plansById.TryGetValue(planId, out var heldPlan)
                    ? heldPlan
                    : null;
            if (plan is null || !plan.Covers(attendedClass))
            {
                notCovered.Add(attendedClass);
                continue;
            }

            var key = (attendedClass.StudentId!.Value, month);
            var used = usedClasses.GetValueOrDefault(key);
            if (plan.ClassCount is { } classesPerMonth && used >= classesPerMonth)
            {
                notCovered.Add(attendedClass);
                continue;
            }

            usedClasses[key] = used + 1;
        }

        return notCovered;
    }

    public static bool HasPlanIn(IEnumerable<StudentPlanChange> studentPlanChanges, BillingMonth month) =>
        StudentPlanTimeline.ChangeIn(studentPlanChanges, month)?.HasPlan == true;
}
