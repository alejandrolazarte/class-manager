using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.Domain.Fees;

public static class FamilyBilling
{
    public static FamilyMonthBilling For(
        long clientId,
        IEnumerable<StudentPlanChange> familyPlanChanges,
        IReadOnlyDictionary<long, ClassPack> plansById,
        IReadOnlyCollection<ClassPackPriceChange> priceChanges,
        BillingMonth month) =>
        new(
            clientId,
            [.. familyPlanChanges
                .GroupBy(change => change.StudentId)
                .Select(studentChanges => StudentPlanTimeline.ChangeIn(studentChanges, month))
                .Where(change => change?.ClassPackId is { } planId && plansById.ContainsKey(planId))
                .Select(change => LineOf(change!, plansById[change!.ClassPackId!.Value], priceChanges, month))
                .OrderBy(line => line.StudentFullName, StringComparer.CurrentCultureIgnoreCase)]);

    private static StudentPlanLine LineOf(
        StudentPlanChange change,
        ClassPack plan,
        IReadOnlyCollection<ClassPackPriceChange> priceChanges,
        BillingMonth month) =>
        new(
            change.StudentId,
            change.Student?.User.FullName ?? string.Empty,
            plan.Id,
            plan.Name,
            change.CustomPrice ?? PlanPrices.PriceIn(plan, priceChanges, month),
            change.CustomPrice);
}
