namespace ClassManager.Core.Domain.ClassPacks;

public sealed record ClassBalance(IReadOnlyList<ClassPackUsage> Purchases, IReadOnlyList<AttendedClass> UnpaidAttendances)
{
    public int AvailableClasses => Purchases
        .Where(usage => usage.Status == ClassPackPurchaseStatus.Active)
        .Sum(usage => usage.RemainingClasses);

    public int UnpaidClasses => UnpaidAttendances.Count;

    public static ClassBalance Calculate(
        IReadOnlyCollection<ClassPackPurchase> purchases,
        IReadOnlyCollection<AttendedClass> attendedClasses,
        DateOnly today)
    {
        var usedClassesByPurchase = purchases.ToDictionary(purchase => purchase, _ => 0);
        var unpaidAttendances = new List<AttendedClass>();

        foreach (var attendedClass in attendedClasses.OrderBy(attended => attended.Date))
        {
            var payingPurchase = purchases
                .Where(purchase => purchase.IsValidOn(attendedClass.Date) && usedClassesByPurchase[purchase] < purchase.ClassCount)
                .OrderBy(purchase => purchase.ExpiresOn ?? DateOnly.MaxValue)
                .ThenBy(purchase => purchase.PurchasedOn)
                .ThenBy(purchase => purchase.CreatedAt)
                .FirstOrDefault();
            if (payingPurchase is null)
            {
                unpaidAttendances.Add(attendedClass);
                continue;
            }

            usedClassesByPurchase[payingPurchase]++;
        }

        var usages = purchases
            .OrderByDescending(purchase => purchase.PurchasedOn)
            .ThenByDescending(purchase => purchase.CreatedAt)
            .Select(purchase => new ClassPackUsage(purchase, usedClassesByPurchase[purchase], StatusOf(purchase, usedClassesByPurchase[purchase], today)))
            .ToList();

        return new ClassBalance(usages, unpaidAttendances);
    }

    private static ClassPackPurchaseStatus StatusOf(ClassPackPurchase purchase, int usedClasses, DateOnly today) =>
        usedClasses >= purchase.ClassCount ? ClassPackPurchaseStatus.UsedUp
        : purchase.IsValidOn(today) ? ClassPackPurchaseStatus.Active
        : ClassPackPurchaseStatus.Expired;
}
