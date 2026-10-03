namespace ClassManager.Core.U.Tests.Domain.Products.When_Product_image_to_remove_does_not_belong_to_it;

public sealed class Then_nothing_is_removed
{
    [Fact]
    public void Then_nothing_is_removed_Run()
    {
        var product = CatalogImageSamples.ProductWithImages(2);

        var removedDocument = product.RemoveImage(Guid.CreateVersion7());

        removedDocument.ShouldBeNull();
        product.ImageCount.ShouldBe(2);
    }
}
