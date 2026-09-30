using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_BrandLogo_is_not_an_image;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var result = BrandLogo.Create("%PDF-1.7 not an image"u8.ToArray(), TestData.Now);

        result.Error!.Code.ShouldBe(BrandErrorCodes.UnsupportedLogo);
    }
}
