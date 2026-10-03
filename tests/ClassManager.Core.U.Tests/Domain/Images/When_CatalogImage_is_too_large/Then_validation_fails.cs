using ClassManager.Core.Domain.Images;

namespace ClassManager.Core.U.Tests.Domain.Images.When_CatalogImage_is_too_large;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        byte[] oversizedImage = [.. CatalogImageSamples.Png, .. new byte[CatalogImage.MaximumSizeInBytes]];

        var image = CatalogImage.Create(oversizedImage);

        image.Error!.Code.ShouldBe(ImageErrorCodes.TooLarge);
    }
}
