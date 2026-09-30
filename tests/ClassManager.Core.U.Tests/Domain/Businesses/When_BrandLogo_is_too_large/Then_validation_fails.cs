using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_BrandLogo_is_too_large;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var content = new byte[BrandLogo.MaximumSizeInBytes + 1];
        BrandLogoSamples.Png.CopyTo(content, 0);

        var result = BrandLogo.Create(content, TestData.Now);

        result.Error!.Code.ShouldBe(BrandErrorCodes.LogoTooLarge);
    }
}
