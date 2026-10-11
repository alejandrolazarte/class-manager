using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_settings_have_unknown_time_zone;

public sealed class Then_nothing_changes
{
    [Fact]
    public void Then_nothing_changes_Run()
    {
        var business = TestData.Business();

        var result = business.UpdateSettings("Panadería Nueva", TestData.Argentina(), "Mars/Olympus_Mons", TestData.CurrencyCode);

        result.Error!.FieldName.ShouldBe(nameof(Business.TimeZoneId));
        business.Name.ShouldBe(TestData.Business().Name);
    }
}
