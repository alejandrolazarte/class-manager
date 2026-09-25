using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_PhoneNumber_is_created_with_international_format;

public sealed class Then_country_code_is_kept
{
    [Fact]
    public void Then_country_code_is_kept_Run()
    {
        var phoneNumber = PhoneNumber.Create("+34 612 34 56 78", TestData.DefaultCountryCallingCode);

        phoneNumber.Value!.Value.ShouldBe("+34612345678");
    }
}
