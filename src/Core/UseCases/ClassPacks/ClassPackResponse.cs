using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.UseCases.Images;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record ClassPackResponse(
    Guid Id,
    string Name,
    string? Description,
    int ClassCount,
    decimal Price,
    int? ValidityMonths,
    bool IsActive,
    int? ClassDurationMinutes,
    IReadOnlyList<Guid> ClassGroupIds,
    IReadOnlyList<CatalogImageResponse> Images)
{
    public static ClassPackResponse From(ClassPack classPack, IDocumentStorageService documentStorage) =>
        new(
            classPack.Id,
            classPack.Name,
            classPack.Description,
            classPack.ClassCount,
            classPack.Price,
            classPack.ValidityMonths,
            classPack.IsActive,
            classPack.ClassDurationMinutes,
            classPack.ClassGroupIds,
            CatalogImageResponse.ListFrom(classPack.ImagesInOrder, documentStorage));
}
