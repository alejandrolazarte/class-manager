using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_is_created_without_a_known_country;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var business = Business.Create(
            Guid.CreateVersion7(), "Demo business", "demo-business", null, TestData.BuenosAiresTimeZoneId, TestData.CurrencyCode, TestData.Now);

        business.Error!.FieldName.ShouldBe(nameof(Business.CountryCode));
    }
}
