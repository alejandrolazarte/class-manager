using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_is_created_with_unknown_time_zone;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var business = Business.Create(Guid.CreateVersion7(), "Demo business", "demo-business", "Mars/Olympus_Mons", TestData.CurrencyCode, TestData.DefaultCountryCallingCode, TestData.Now);

        business.Error!.FieldName.ShouldBe(nameof(Business.TimeZoneId));
    }
}
