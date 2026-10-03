using ClassManager.Core.Domain.Images;
using ClassManager.Storage.Images;

namespace ClassManager.Core.U.Tests.Domain.Images.When_CatalogImage_is_a_jpeg;

public sealed class Then_its_format_is_jpeg
{
    [Fact]
    public void Then_its_format_is_jpeg_Run()
    {
        var image = CatalogImage.Create(CatalogImageSamples.Jpeg);

        image.Value!.Format.ShouldBe(ImageFormat.Jpeg);
    }
}
