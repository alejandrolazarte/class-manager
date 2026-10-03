using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.UseCases.Images;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record ClassPackResponse(
    Guid Id,
    string Name,
    int ClassCount,
    decimal Price,
    int? ValidityMonths,
    bool IsActive,
    int? ClassDurationMinutes,
    string? MaterialUrl,
    IReadOnlyList<Guid> ClassGroupIds,
    IReadOnlyList<CatalogImageResponse> Images)
{
    public static ClassPackResponse From(ClassPack classPack, IDocumentStorageService documentStorage) =>
        new(
            classPack.Id,
            classPack.Name,
            classPack.ClassCount,
            classPack.Price,
            classPack.ValidityMonths,
            classPack.IsActive,
            classPack.ClassDurationMinutes,
            classPack.MaterialUrl,
            classPack.ClassGroupIds,
            CatalogImageResponse.ListFrom(classPack.ImagesInOrder, documentStorage));
}
