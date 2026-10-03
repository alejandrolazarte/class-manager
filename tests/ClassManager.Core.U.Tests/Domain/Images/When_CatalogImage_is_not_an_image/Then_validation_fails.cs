using ClassManager.Core.Domain.Images;

namespace ClassManager.Core.U.Tests.Domain.Images.When_CatalogImage_is_not_an_image;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var image = CatalogImage.Create(CatalogImageSamples.PlainText);

        image.Error!.Code.ShouldBe(ImageErrorCodes.Unsupported);
    }
}
