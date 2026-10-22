using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record ClassPackPurchaseResponse(
    long Id,
    long ClientId,
    string Name,
    int ClassCount,
    decimal Price,
    string CurrencyCode,
    DateOnly PurchasedOn,
    DateOnly? ExpiresOn,
    PaymentMethod? Method,
    string? Notes,
    int? ClassDurationMinutes,
    long? TrialLessonId)
{
    public static ClassPackPurchaseResponse From(ClassPackPurchase purchase) =>
        new(
            purchase.Id,
            purchase.ClientId,
            purchase.Name,
            purchase.ClassCount,
            purchase.Price,
            purchase.CurrencyCode,
            purchase.PurchasedOn,
            purchase.ExpiresOn,
            purchase.Method,
            purchase.Notes,
            purchase.ClassDurationMinutes,
            purchase.TrialLessonId);
}
