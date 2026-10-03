using ClassManager.Core.Domain.Images;

namespace ClassManager.Core.Abstractions.Storage;

public interface ICatalogImageService
{
    Task<Uri> SaveAsync(CatalogImageOwner owner, Guid ownerId, CatalogImage image, CancellationToken cancellationToken);

    Task DeleteAsync(string? imageUrl, CancellationToken cancellationToken);
}
