namespace ClassManager.Core.U.Tests.Services.CatalogImages.When_CatalogImageService_removes_a_photo;

public sealed class Then_its_document_and_file_are_deleted
{
    [Fact]
    public async Task Then_its_document_and_file_are_deleted_Run()
    {
        var builder = new CatalogImageServiceBuilder();
        var product = CatalogImageSamples.ProductWithImages(2);
        var removedDocument = product.ImagesInOrder[0];

        var result = await builder.Build().RemoveAsync(product, removedDocument.Id, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        builder.DocumentRepository.Verify(repository => repository.Remove(removedDocument), Times.Once);
        builder.DocumentStorage.Verify(storage => storage.DeleteFileAsync(removedDocument, It.IsAny<CancellationToken>()), Times.Once);
    }
}
