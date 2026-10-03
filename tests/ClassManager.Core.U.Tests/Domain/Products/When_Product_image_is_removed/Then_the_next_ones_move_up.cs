namespace ClassManager.Core.U.Tests.Domain.Products.When_Product_image_is_removed;

public sealed class Then_the_next_ones_move_up
{
    [Fact]
    public void Then_the_next_ones_move_up_Run()
    {
        var product = CatalogImageSamples.ProductWithImages(3);
        var documents = product.ImagesInOrder;

        product.RemoveImage(documents[0].Id);

        product.ImagesInOrder.ShouldBe([documents[1], documents[2]]);
        product.Images.Select(image => image.Position).Order().ShouldBe([0, 1]);
    }
}
