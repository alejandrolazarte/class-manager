using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_is_created_with_unknown_currency;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var business = Business.Create(TestIds.Next(), "Demo business", "demo-business", TestData.Argentina(), TestData.BuenosAiresTimeZoneId, "ABC", TestData.Now);

        business.Error!.FieldName.ShouldBe(nameof(Business.CurrencyCode));
    }
}
