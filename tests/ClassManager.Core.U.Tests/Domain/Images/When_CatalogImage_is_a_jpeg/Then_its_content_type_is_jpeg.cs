using ClassManager.Core.Domain.Images;

namespace ClassManager.Core.U.Tests.Domain.Images.When_CatalogImage_is_a_jpeg;

public sealed class Then_its_content_type_is_jpeg
{
    [Fact]
    public void Then_its_content_type_is_jpeg_Run()
    {
        var image = CatalogImage.Validate(CatalogImageSamples.Jpeg);

        image.Value!.ContentType.ShouldBe("image/jpeg");
        image.Value.FileExtension.ShouldBe(".jpg");
    }
}
