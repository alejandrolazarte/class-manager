namespace ClassManager.Core.Domain.ClassPacks;

public sealed record ClassPackUsage(ClassPackPurchase Purchase, int UsedClasses, ClassPackPurchaseStatus Status)
{
    public int RemainingClasses => Purchase.ClassCount - UsedClasses;
}
