using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Images;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record ClassPackPriceChangeResponse(string EffectiveFrom, decimal Price);

public sealed record ClassPackResponse(
    long Id,
    string Name,
    string? Description,
    int? ClassCount,
    decimal Price,
    int? ValidityMonths,
    bool IsActive,
    int? ClassDurationMinutes,
    IReadOnlyList<long> ClassGroupIds,
    IReadOnlyList<CatalogImageResponse> Images,
    bool CoversAnyGroupClass,
    bool RenewsMonthly,
    bool AppliesFamilyDiscount,
    IReadOnlyList<ClassPackPriceChangeResponse> PriceChanges)
{
    public static ClassPackResponse From(
        ClassPack classPack,
        IDocumentStorageService documentStorage,
        IReadOnlyCollection<ClassPackPriceChange> priceChanges,
        DateOnly today) =>
        new(
            classPack.Id,
            classPack.Name,
            classPack.Description,
            classPack.ClassCount,
            classPack.RenewsMonthly ? PlanPrices.PriceIn(classPack, priceChanges, BillingMonth.From(today)) : classPack.Price,
            classPack.ValidityMonths,
            classPack.IsActive,
            classPack.ClassDurationMinutes,
            classPack.ClassGroupIds,
            CatalogImageResponse.ListFrom(classPack.ImagesInOrder, documentStorage),
            classPack.CoversAnyGroupClass,
            classPack.RenewsMonthly,
            classPack.AppliesFamilyDiscount,
            classPack.RenewsMonthly
                ? [.. priceChanges
                    .Where(change => change.ClassPackId == classPack.Id)
                    .OrderBy(change => change.EffectiveFrom)
                    .Select(change => new ClassPackPriceChangeResponse(BillingMonth.From(change.EffectiveFrom).ToString(), change.Price))]
                : []);
}
