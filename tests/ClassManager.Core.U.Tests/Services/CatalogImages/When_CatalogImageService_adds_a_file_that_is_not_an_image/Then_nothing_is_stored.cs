using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Images;

namespace ClassManager.Core.U.Tests.Services.CatalogImages.When_CatalogImageService_adds_a_file_that_is_not_an_image;

public sealed class Then_nothing_is_stored
{
    [Fact]
    public async Task Then_nothing_is_stored_Run()
    {
        var builder = new CatalogImageServiceBuilder();
        var product = CatalogImageSamples.ProductWithImages(0);

        var result = await builder.Build().AddAsync(product, DocumentOwner.Product, CatalogImageSamples.PlainText, CancellationToken.None);

        result.Error!.Code.ShouldBe(ImageErrorCodes.Unsupported);
        product.ImageCount.ShouldBe(0);
    }
}
