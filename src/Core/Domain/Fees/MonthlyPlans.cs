using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.Domain.Fees;

public sealed record MonthlyPlans(
    IReadOnlyList<StudentPlanChange> PlanChanges,
    IReadOnlyDictionary<long, ClassPack> PlansById,
    IReadOnlyList<ClassPackPriceChange> PriceChanges)
{
    public IEnumerable<StudentPlanChange> OfFamily(long clientId) =>
        PlanChanges.Where(change => change.Student?.ClientId == clientId);
}
