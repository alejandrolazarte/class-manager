using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_PhoneNumber_is_created_with_local_format;

public sealed class Then_it_is_normalized_to_E164
{
    [Fact]
    public void Then_it_is_normalized_to_E164_Run()
    {
        var phoneNumber = PhoneNumber.Create("(011) 2233-4455", TestData.DefaultCountryCallingCode);

        phoneNumber.Value!.Value.ShouldBe(TestData.NormalizedClientPhoneNumber);
    }
}
