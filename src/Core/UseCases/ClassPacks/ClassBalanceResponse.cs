using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record ClassPackUsageResponse(
    Guid Id,
    string Name,
    int ClassCount,
    decimal Price,
    DateOnly PurchasedOn,
    DateOnly? ExpiresOn,
    PaymentMethod Method,
    int UsedClasses,
    int RemainingClasses,
    ClassPackPurchaseStatus Status)
{
    public static ClassPackUsageResponse From(ClassPackUsage usage) =>
        new(
            usage.Purchase.Id,
            usage.Purchase.Name,
            usage.Purchase.ClassCount,
            usage.Purchase.Price,
            usage.Purchase.PurchasedOn,
            usage.Purchase.ExpiresOn,
            usage.Purchase.Method,
            usage.UsedClasses,
            usage.RemainingClasses,
            usage.Status);
}

public sealed record AttendedClassResponse(DateOnly Date, string StudentFullName, string ClassGroupName, bool IsPrivateLesson)
{
    public static AttendedClassResponse From(AttendedClass attendedClass) =>
        new(attendedClass.Date, attendedClass.StudentFullName, attendedClass.ClassGroupName, attendedClass.IsPrivateLesson);
}

public sealed record ClassBalanceResponse(
    int AvailableClasses,
    int UnpaidClasses,
    IReadOnlyList<ClassPackUsageResponse> Purchases,
    IReadOnlyList<AttendedClassResponse> UnpaidAttendances)
{
    public static ClassBalanceResponse From(ClassBalance balance) =>
        new(
            balance.AvailableClasses,
            balance.UnpaidClasses,
            [.. balance.Purchases.Select(ClassPackUsageResponse.From)],
            [.. balance.UnpaidAttendances.Select(AttendedClassResponse.From)]);
}
