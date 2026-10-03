using ClassManager.Core.Domain.Documents;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Core.U.Tests.Services.CatalogImages.When_CatalogImageService_product_reached_the_plan_limit;

public sealed class Then_nothing_is_stored
{
    [Fact]
    public async Task Then_nothing_is_stored_Run()
    {
        var builder = new CatalogImageServiceBuilder(photoLimit: 1);
        var product = CatalogImageSamples.ProductWithImages(1);

        var result = await builder.Build().AddAsync(product, DocumentOwner.Product, CatalogImageSamples.Png, CancellationToken.None);

        result.Error!.Code.ShouldBe(FeatureErrorCodes.LimitReached);
        builder.DocumentStorage.Verify(
            storage => storage.StoreAsync(
                It.IsAny<DocumentOwner>(), It.IsAny<Guid>(), It.IsAny<DocumentContent>(), It.IsAny<DocumentVisibility>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
