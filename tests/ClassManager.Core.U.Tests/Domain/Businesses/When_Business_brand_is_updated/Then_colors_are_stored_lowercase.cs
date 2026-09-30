namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_brand_is_updated;

public sealed class Then_colors_are_stored_lowercase
{
    [Fact]
    public void Then_colors_are_stored_lowercase_Run()
    {
        var business = TestData.Business();

        var result = business.UpdateBrand("  Club Delta  ", "#0076B4", "#EFB062", locksTheme: true);

        result.IsSuccess.ShouldBeTrue();
        business.ThemeColor.ShouldBe("#0076b4");
        business.AccentColor.ShouldBe("#efb062");
        business.BrandName.ShouldBe("Club Delta");
        business.LocksTheme.ShouldBeTrue();
    }
}
