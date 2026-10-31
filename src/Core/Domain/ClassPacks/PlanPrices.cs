using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.Domain.ClassPacks;

public static class PlanPrices
{
    public static decimal PriceIn(ClassPack plan, IEnumerable<ClassPackPriceChange> priceChanges, BillingMonth month) =>
        priceChanges
            .Where(change => change.ClassPackId == plan.Id && change.EffectiveFrom <= month.FirstDay)
            .MaxBy(change => change.EffectiveFrom)?
            .Price
        ?? plan.Price;
}
