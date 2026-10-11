using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_time_zone_is_from_another_country;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var business = Business.Create(
            Guid.CreateVersion7(), "Demo business", "demo-business", TestData.Spain(), TestData.BuenosAiresTimeZoneId, TestData.EuroCurrencyCode, TestData.Now);

        business.Error!.FieldName.ShouldBe(nameof(Business.TimeZoneId));
    }
}
