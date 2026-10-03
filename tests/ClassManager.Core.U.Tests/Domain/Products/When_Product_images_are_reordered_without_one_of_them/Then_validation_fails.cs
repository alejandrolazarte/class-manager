using ClassManager.Core.Domain.Images;

namespace ClassManager.Core.U.Tests.Domain.Products.When_Product_images_are_reordered_without_one_of_them;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var product = CatalogImageSamples.ProductWithImages(2);

        var result = product.ReorderImages([product.ImagesInOrder[1].Id]);

        result.Error!.Code.ShouldBe(ImageErrorCodes.OrderMismatch);
    }
}
