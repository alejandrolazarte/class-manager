using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.U.Tests.Services.CatalogImages.When_CatalogImageService_adds_a_photo;

public sealed class Then_it_is_public_and_last
{
    [Fact]
    public async Task Then_it_is_public_and_last_Run()
    {
        var builder = new CatalogImageServiceBuilder();
        var product = CatalogImageSamples.ProductWithImages(1);

        await builder.Build().AddAsync(product, DocumentOwner.Product, CatalogImageSamples.Png, CancellationToken.None);

        product.ImagesInOrder[^1].ShouldBe(builder.StoredDocument);
        builder.DocumentStorage.Verify(storage => storage.StoreAsync(
            DocumentOwner.Product, product.Id, It.IsAny<DocumentContent>(), DocumentVisibility.Public, It.IsAny<CancellationToken>()));
        builder.DocumentRepository.Verify(repository => repository.Add(builder.StoredDocument));
    }
}
