using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_is_created_with_unknown_currency;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var business = Business.Create("Demo business", "demo-business", TestData.BuenosAiresTimeZoneId, "ABC", TestData.DefaultCountryCallingCode, TestData.Now);

        business.Error!.FieldName.ShouldBe(nameof(Business.CurrencyCode));
    }
}
