namespace ClassManager.Core.U.Tests.Domain.Products.When_Product_images_are_reordered;

public sealed class Then_the_first_one_is_the_main_photo
{
    [Fact]
    public void Then_the_first_one_is_the_main_photo_Run()
    {
        var product = CatalogImageSamples.ProductWithImages(3);
        var documents = product.ImagesInOrder;

        var result = product.ReorderImages([documents[2].Id, documents[0].Id, documents[1].Id]);

        result.IsSuccess.ShouldBeTrue();
        product.ImagesInOrder.ShouldBe([documents[2], documents[0], documents[1]]);
    }
}
