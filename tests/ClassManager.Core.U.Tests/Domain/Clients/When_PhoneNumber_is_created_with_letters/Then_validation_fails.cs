using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_PhoneNumber_is_created_with_letters;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var phoneNumber = PhoneNumber.Create("11 2233-ABCD", TestData.DefaultCountryCallingCode);

        phoneNumber.Error!.Kind.ShouldBe(ErrorKind.Validation);
    }
}
