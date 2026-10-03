using ClassManager.Core.Domain.Images;
using ClassManager.Core.UseCases.Products;

namespace ClassManager.Core.U.Tests.UseCases.Products.When_SetProductImage_file_is_not_an_image;

public sealed class Then_nothing_is_stored
{
    [Fact]
    public async Task Then_nothing_is_stored_Run()
    {
        var builder = new ProductImageUseCaseBuilder();

        var result = await builder.BuildSetUseCase().ExecuteAsync(
            new SetProductImageCommand(builder.Product.Id, CatalogImageSamples.PlainText), CancellationToken.None);

        result.Error!.Code.ShouldBe(ImageErrorCodes.Unsupported);
        builder.CatalogImageService.Verify(
            service => service.SaveAsync(It.IsAny<CatalogImageOwner>(), It.IsAny<Guid>(), It.IsAny<CatalogImage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
