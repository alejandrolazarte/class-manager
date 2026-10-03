using ClassManager.Core.UseCases.Products;

namespace ClassManager.Core.U.Tests.UseCases.Products.When_SetProductImage_cannot_save_the_product;

public sealed class Then_the_new_image_is_deleted
{
    [Fact]
    public async Task Then_the_new_image_is_deleted_Run()
    {
        var builder = new ProductImageUseCaseBuilder();
        builder.UnitOfWork.Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException());

        await Should.ThrowAsync<InvalidOperationException>(() => builder.BuildSetUseCase().ExecuteAsync(
            new SetProductImageCommand(builder.Product.Id, CatalogImageSamples.Png), CancellationToken.None));

        builder.CatalogImageService.Verify(
            service => service.DeleteAsync(ProductImageUseCaseBuilder.NewImageUrl.AbsoluteUri, It.IsAny<CancellationToken>()), Times.Once);
    }
}
