namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_brand_name_is_blank;

public sealed class Then_business_name_is_shown
{
    [Fact]
    public void Then_business_name_is_shown_Run()
    {
        var business = TestData.Business();

        business.UpdateBrand("   ", themeColor: null, accentColor: null, locksTheme: false);

        business.BrandName.ShouldBeNull();
        business.BrandDisplayName.ShouldBe(business.Name);
    }
}
