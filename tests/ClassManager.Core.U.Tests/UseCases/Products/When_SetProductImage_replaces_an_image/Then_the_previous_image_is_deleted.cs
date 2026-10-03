using ClassManager.Core.UseCases.Products;

namespace ClassManager.Core.U.Tests.UseCases.Products.When_SetProductImage_replaces_an_image;

public sealed class Then_the_previous_image_is_deleted
{
    [Fact]
    public async Task Then_the_previous_image_is_deleted_Run()
    {
        var builder = new ProductImageUseCaseBuilder();
        builder.Product.ChangeImage(new Uri(CatalogImageSamples.PreviousImageUrl));

        var result = await builder.BuildSetUseCase().ExecuteAsync(
            new SetProductImageCommand(builder.Product.Id, CatalogImageSamples.Png), CancellationToken.None);

        result.Value!.ImageUrl.ShouldBe(ProductImageUseCaseBuilder.NewImageUrl.AbsoluteUri);
        builder.CatalogImageService.Verify(service => service.DeleteAsync(CatalogImageSamples.PreviousImageUrl, It.IsAny<CancellationToken>()), Times.Once);
    }
}
