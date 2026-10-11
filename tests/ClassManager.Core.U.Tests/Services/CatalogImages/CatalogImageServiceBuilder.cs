using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Services;

namespace ClassManager.Core.U.Tests.Services.CatalogImages;

internal sealed class CatalogImageServiceBuilder
{
    public CatalogImageServiceBuilder()
    {
        DocumentStorage
            .Setup(storage => storage.StoreAsync(
                It.IsAny<DocumentOwner>(), It.IsAny<Guid>(), It.IsAny<DocumentContent>(), It.IsAny<DocumentVisibility>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(StoredDocument);
    }

    public Document StoredDocument { get; } = CatalogImageSamples.PublicDocument();

    public Mock<IDocumentStorageService> DocumentStorage { get; } = new();

    public Mock<IDocumentRepository> DocumentRepository { get; } = new();

    public Mock<IUnitOfWork> UnitOfWork { get; } = new();

    public CatalogImageService Build() => new(DocumentStorage.Object, DocumentRepository.Object, UnitOfWork.Object);
}
