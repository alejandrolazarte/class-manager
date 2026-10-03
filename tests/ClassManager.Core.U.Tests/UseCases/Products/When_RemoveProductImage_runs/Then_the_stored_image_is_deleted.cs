using ClassManager.Core.UseCases.Products;

namespace ClassManager.Core.U.Tests.UseCases.Products.When_RemoveProductImage_runs;

public sealed class Then_the_stored_image_is_deleted
{
    [Fact]
    public async Task Then_the_stored_image_is_deleted_Run()
    {
        var builder = new ProductImageUseCaseBuilder();
        builder.Product.ChangeImage(new Uri(CatalogImageSamples.PreviousImageUrl));

        var result = await builder.BuildRemoveUseCase().ExecuteAsync(new RemoveProductImageCommand(builder.Product.Id), CancellationToken.None);

        result.Value!.ImageUrl.ShouldBeNull();
        builder.CatalogImageService.Verify(service => service.DeleteAsync(CatalogImageSamples.PreviousImageUrl, It.IsAny<CancellationToken>()), Times.Once);
    }
}
