using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_brand_is_locked_without_main_color;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var business = TestData.Business();

        var result = business.UpdateBrand(brandName: null, themeColor: null, accentColor: "#efb062", locksTheme: true);

        result.Error!.FieldName.ShouldBe(nameof(Business.LocksTheme));
        business.LocksTheme.ShouldBeFalse();
    }
}
