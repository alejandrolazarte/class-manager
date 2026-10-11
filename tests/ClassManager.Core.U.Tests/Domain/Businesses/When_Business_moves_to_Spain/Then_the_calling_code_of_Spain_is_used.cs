namespace ClassManager.Core.U.Tests.Domain.Businesses.When_Business_moves_to_Spain;

public sealed class Then_the_calling_code_of_Spain_is_used
{
    [Fact]
    public void Then_the_calling_code_of_Spain_is_used_Run()
    {
        var business = TestData.Business();

        business.UpdateSettings(business.Name, TestData.Spain(), TestData.MadridTimeZoneId, TestData.EuroCurrencyCode);

        business.DefaultCountryCallingCode.ShouldBe(TestData.SpainCallingCode);
        business.CountryCode.ShouldBe(TestData.SpainCountryCode);
    }
}
