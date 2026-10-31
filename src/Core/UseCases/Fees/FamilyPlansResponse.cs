using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.UseCases.Fees;

public sealed record StudentPlanChangeResponse(string EffectiveFrom, long? ClassPackId, string? PlanName, decimal? CustomPrice);

public sealed record StudentPlanResponse(
    long StudentId,
    string StudentFullName,
    long? ClassPackId,
    string? PlanName,
    decimal? Price,
    decimal? CustomPrice,
    string? Since,
    IReadOnlyList<StudentPlanChangeResponse> UpcomingChanges);

public sealed record FamilyPlansResponse(decimal? MonthlyDue, IReadOnlyList<StudentPlanResponse> Students)
{
    public static FamilyPlansResponse From(
        long clientId,
        IEnumerable<Student> students,
        IReadOnlyCollection<StudentPlanChange> planChanges,
        IReadOnlyCollection<ClassPack> catalog,
        IReadOnlyCollection<ClassPackPriceChange> priceChanges,
        DateOnly today)
    {
        var month = BillingMonth.From(today);
        var plansById = catalog.Where(classPack => classPack.RenewsMonthly).ToDictionary(classPack => classPack.Id);
        var studentPlans = students
            .OrderBy(student => student.User.FullName, StringComparer.CurrentCultureIgnoreCase)
            .Select(student => StudentPlanOf(student, [.. planChanges.Where(change => change.StudentId == student.Id)], plansById, priceChanges, month))
            .ToList();
        var due = FamilyBilling.For(clientId, planChanges, plansById, priceChanges, month).Due;
        return new FamilyPlansResponse(due, studentPlans);
    }

    private static StudentPlanResponse StudentPlanOf(
        Student student,
        IReadOnlyCollection<StudentPlanChange> studentChanges,
        IReadOnlyDictionary<long, ClassPack> plansById,
        IReadOnlyCollection<ClassPackPriceChange> priceChanges,
        BillingMonth month)
    {
        var current = StudentPlanTimeline.ChangeIn(studentChanges, month);
        var plan = current?.ClassPackId is { } planId ? plansById.GetValueOrDefault(planId) : null;
        var upcomingChanges = studentChanges
            .Where(change => change.EffectiveFrom > month.FirstDay)
            .OrderBy(change => change.EffectiveFrom)
            .Select(change => new StudentPlanChangeResponse(
                BillingMonth.From(change.EffectiveFrom).ToString(),
                change.ClassPackId,
                change.ClassPackId is { } changePlanId ? plansById.GetValueOrDefault(changePlanId)?.Name : null,
                change.CustomPrice))
            .ToList();
        return new StudentPlanResponse(
            student.Id,
            student.User.FullName,
            plan?.Id,
            plan?.Name,
            plan is null ? null : current!.CustomPrice ?? PlanPrices.PriceIn(plan, priceChanges, month),
            plan is null ? null : current!.CustomPrice,
            current is null ? null : BillingMonth.From(current.EffectiveFrom).ToString(),
            upcomingChanges);
    }
}
