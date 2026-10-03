using ClassManager.Core.Domain.Images;

namespace ClassManager.Core.U.Tests.Services.CatalogImages.When_CatalogImageService_removes_a_photo_of_another_product;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        var builder = new CatalogImageServiceBuilder();
        var product = CatalogImageSamples.ProductWithImages(1);

        var result = await builder.Build().RemoveAsync(product, Guid.CreateVersion7(), CancellationToken.None);

        result.Error!.Code.ShouldBe(ImageErrorCodes.NotFound);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
