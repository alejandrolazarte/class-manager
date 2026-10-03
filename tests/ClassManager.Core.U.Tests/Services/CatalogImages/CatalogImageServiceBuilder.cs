using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.Services;
using ClassManager.Subscriptions.Access;
using ClassManager.Subscriptions.Catalog;

namespace ClassManager.Core.U.Tests.Services.CatalogImages;

internal sealed class CatalogImageServiceBuilder
{
    public CatalogImageServiceBuilder(int photoLimit = 5)
    {
        var features = EffectiveFeatures.Combine(
            PlanCodes.Pro,
            [PlanFeature.Create(PlanCodes.Pro, Features.CatalogPhotos, photoLimit)],
            [],
            DateOnly.FromDateTime(TestData.Now.UtcDateTime));
        FeatureAccess.Setup(access => access.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(features);
        DocumentStorage
            .Setup(storage => storage.StoreAsync(
                It.IsAny<DocumentOwner>(), It.IsAny<Guid>(), It.IsAny<DocumentContent>(), It.IsAny<DocumentVisibility>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(StoredDocument);
    }

    public Document StoredDocument { get; } = CatalogImageSamples.PublicDocument();

    public Mock<IFeatureAccess> FeatureAccess { get; } = new();

    public Mock<IDocumentStorageService> DocumentStorage { get; } = new();

    public Mock<IDocumentRepository> DocumentRepository { get; } = new();

    public Mock<IUnitOfWork> UnitOfWork { get; } = new();

    public CatalogImageService Build() => new(FeatureAccess.Object, DocumentStorage.Object, DocumentRepository.Object, UnitOfWork.Object);
}
