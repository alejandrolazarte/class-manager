using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record ClassPackPurchaseResponse(
    Guid Id,
    Guid ClientId,
    string Name,
    int ClassCount,
    decimal Price,
    DateOnly PurchasedOn,
    DateOnly? ExpiresOn,
    PaymentMethod Method,
    string? Notes,
    int? ClassDurationMinutes,
    Guid? TrialLessonId)
{
    public static ClassPackPurchaseResponse From(ClassPackPurchase purchase) =>
        new(
            purchase.Id,
            purchase.ClientId,
            purchase.Name,
            purchase.ClassCount,
            purchase.Price,
            purchase.PurchasedOn,
            purchase.ExpiresOn,
            purchase.Method,
            purchase.Notes,
            purchase.ClassDurationMinutes,
            purchase.TrialLessonId);
}
