using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.U.Tests.Services.CatalogImages.When_CatalogImageService_cannot_save_the_new_photo;

public sealed class Then_its_file_is_deleted
{
    [Fact]
    public async Task Then_its_file_is_deleted_Run()
    {
        var builder = new CatalogImageServiceBuilder();
        builder.UnitOfWork.Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException());
        var product = CatalogImageSamples.ProductWithImages(0);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            builder.Build().AddAsync(product, DocumentOwner.Product, CatalogImageSamples.Png, CancellationToken.None));

        builder.DocumentStorage.Verify(storage => storage.DeleteFileAsync(builder.StoredDocument, It.IsAny<CancellationToken>()), Times.Once);
    }
}
