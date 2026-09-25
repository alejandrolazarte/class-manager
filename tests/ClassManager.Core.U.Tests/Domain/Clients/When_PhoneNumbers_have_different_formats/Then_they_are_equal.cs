using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_PhoneNumbers_have_different_formats;

public sealed class Then_they_are_equal
{
    [Fact]
    public void Then_they_are_equal_Run()
    {
        var localPhoneNumber = PhoneNumber.Create(TestData.ClientPhoneNumber, TestData.DefaultCountryCallingCode).Value;
        var internationalPhoneNumber = PhoneNumber.Create(TestData.NormalizedClientPhoneNumber, TestData.DefaultCountryCallingCode).Value;

        localPhoneNumber.ShouldBe(internationalPhoneNumber);
    }
}
