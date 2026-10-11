using ClassManager.Core.Abstractions.Images;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Images;

namespace ClassManager.Core.Services;

public sealed class CatalogImageService(
    IDocumentStorageService documentStorage,
    IDocumentRepository documentRepository,
    IUnitOfWork unitOfWork)
    : ICatalogImageService
{
    private const string ImageNotFoundMessage = "The photo does not exist.";
    private const string ImageLimitReachedMessage = "No more photos can be added here.";

    public async Task<Result> AddAsync(IHasCatalogImages imageHolder, DocumentOwner owner, byte[] content, CancellationToken cancellationToken)
    {
        var image = CatalogImage.Validate(content);
        if (image.IsFailure)
        {
            return Result.Failure(image.Error!);
        }

        if (imageHolder.ImageCount >= imageHolder.MaximumImageCount)
        {
            return Result.Conflict(ImageLimitReachedMessage, ImageErrorCodes.LimitReached);
        }

        var document = await documentStorage.StoreAsync(owner, imageHolder.Id, image.Value!, DocumentVisibility.Public, cancellationToken);
        documentRepository.Add(document);
        imageHolder.AddImage(document);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await documentStorage.DeleteFileAsync(document, CancellationToken.None);
            throw;
        }

        return Result.Success();
    }

    public async Task<Result> RemoveAsync(IHasCatalogImages imageHolder, Guid documentId, CancellationToken cancellationToken)
    {
        var document = imageHolder.RemoveImage(documentId);
        if (document is null)
        {
            return Result.NotFound(ImageNotFoundMessage, ImageErrorCodes.NotFound);
        }

        documentRepository.Remove(document);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await documentStorage.DeleteFileAsync(document, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ReorderAsync(IHasCatalogImages imageHolder, IReadOnlyList<Guid>? documentIds, CancellationToken cancellationToken)
    {
        var reorder = imageHolder.ReorderImages(documentIds);
        if (reorder.IsFailure)
        {
            return reorder;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
