using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Images;

namespace ClassManager.Core.UseCases.Images;

internal static class CatalogImageChanges
{
    public static async Task<Result> ReplaceAsync(
        IHasCatalogImage imageHolder,
        CatalogImageOwner owner,
        byte[] content,
        ICatalogImageService catalogImageService,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var image = CatalogImage.Create(content);
        if (image.IsFailure)
        {
            return Result.Failure(image.Error!);
        }

        var previousImageUrl = imageHolder.ImageUrl;
        var newImageUrl = await catalogImageService.SaveAsync(owner, imageHolder.Id, image.Value!, cancellationToken);
        imageHolder.ChangeImage(newImageUrl);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await catalogImageService.DeleteAsync(newImageUrl.AbsoluteUri, CancellationToken.None);
            throw;
        }

        await catalogImageService.DeleteAsync(previousImageUrl, cancellationToken);
        return Result.Success();
    }

    public static async Task RemoveAsync(
        IHasCatalogImage imageHolder,
        ICatalogImageService catalogImageService,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var previousImageUrl = imageHolder.ImageUrl;
        imageHolder.RemoveImage();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await catalogImageService.DeleteAsync(previousImageUrl, cancellationToken);
    }
}
