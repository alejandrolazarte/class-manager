using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_brand_color_is_malformed;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var business = TestData.Business();

        var result = business.UpdateBrand(brandName: null, themeColor: "blue", accentColor: null, locksTheme: false);

        result.Error!.FieldName.ShouldBe(nameof(Business.ThemeColor));
        business.ThemeColor.ShouldBeNull();
    }
}
