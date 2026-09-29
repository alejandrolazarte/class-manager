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
    ClassPackPurchaseStatus Status,
    int? ClassDurationMinutes,
    string? MaterialUrl,
    Guid? RecordedByUserId = null)
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
            usage.Status,
            usage.Purchase.ClassDurationMinutes,
            usage.Purchase.MaterialUrl,
            usage.Purchase.RecordedByUserId);
}

public sealed record AttendedClassResponse(DateOnly Date, string StudentFullName, string ClassGroupName, bool IsPrivateLesson)
{
    public static AttendedClassResponse From(AttendedClass attendedClass) =>
        new(attendedClass.Date, attendedClass.StudentFullName, attendedClass.ClassGroupName, attendedClass.IsPrivateLesson);
}

public sealed record DeductibleTrialResponse(Guid PrivateLessonId, DateOnly Date, string StudentFullName, decimal TrialPrice);

public sealed record ClassBalanceResponse(
    int AvailableClasses,
    int UnpaidClasses,
    IReadOnlyList<ClassPackUsageResponse> Purchases,
    IReadOnlyList<AttendedClassResponse> UnpaidAttendances,
    IReadOnlyList<DeductibleTrialResponse> DeductibleTrials)
{
    public static ClassBalanceResponse From(ClassBalance balance, IReadOnlyList<DeductibleTrialResponse> deductibleTrials) =>
        new(
            balance.AvailableClasses,
            balance.UnpaidClasses,
            [.. balance.Purchases.Select(ClassPackUsageResponse.From)],
            [.. balance.UnpaidAttendances.Select(AttendedClassResponse.From)],
            deductibleTrials);
}
