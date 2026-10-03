using ClassManager.Core.Abstractions.Images;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Images;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Core.Services;

public sealed class CatalogImageService(
    IFeatureAccess featureAccess,
    IDocumentStorageService documentStorage,
    IDocumentRepository documentRepository,
    IUnitOfWork unitOfWork)
    : ICatalogImageService
{
    private const string ImageNotFoundMessage = "The photo does not exist.";

    public async Task<Result> AddAsync(IHasCatalogImages imageHolder, DocumentOwner owner, byte[] content, CancellationToken cancellationToken)
    {
        var image = CatalogImage.Validate(content);
        if (image.IsFailure)
        {
            return Result.Failure(image.Error!);
        }

        var features = await featureAccess.GetCurrentAsync(cancellationToken);
        if (!features.AllowsAnother(Features.CatalogPhotos, imageHolder.ImageCount))
        {
            return Result.Failure(FeatureErrors.LimitReached(Features.CatalogPhotos));
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
