namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_settings_are_updated;

public sealed class Then_slug_does_not_change
{
    [Fact]
    public void Then_slug_does_not_change_Run()
    {
        var business = TestData.Business();
        var originalSlug = business.Slug;

        var result = business.UpdateSettings("Panadería Nueva", "Europe/Madrid", "eur", "34");

        result.IsSuccess.ShouldBeTrue();
        business.Slug.ShouldBe(originalSlug);
        business.CurrencyCode.ShouldBe("EUR");
    }
}
