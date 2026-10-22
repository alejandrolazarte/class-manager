using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Images;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Services.CatalogImages.When_CatalogImageService_product_has_the_most_photos_allowed;

public sealed class Then_nothing_is_stored
{
    [Fact]
    public async Task Then_nothing_is_stored_Run()
    {
        var builder = new CatalogImageServiceBuilder();
        var product = CatalogImageSamples.ProductWithImages(Product.MaximumImages);

        var result = await builder.Build().AddAsync(product, DocumentOwner.Product, CatalogImageSamples.Png, CancellationToken.None);

        result.Error!.Code.ShouldBe(ImageErrorCodes.LimitReached);
        builder.DocumentStorage.Verify(
            storage => storage.StoreAsync(
                It.IsAny<DocumentOwner>(), It.IsAny<long>(), It.IsAny<DocumentContent>(), It.IsAny<DocumentVisibility>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
