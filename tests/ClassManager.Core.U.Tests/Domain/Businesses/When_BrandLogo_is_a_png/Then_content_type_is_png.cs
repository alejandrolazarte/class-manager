using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_BrandLogo_is_a_png;

public sealed class Then_content_type_is_png
{
    [Fact]
    public void Then_content_type_is_png_Run()
    {
        var result = BrandLogo.Create(BrandLogoSamples.Png, TestData.Now);

        result.Value!.ContentType.ShouldBe(BrandLogo.PngContentType);
    }
}
