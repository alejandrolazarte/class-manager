using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_PhoneNumber_from_Argentina_is_created_in_a_Spanish_business;

public sealed class Then_the_Argentine_code_is_kept
{
    [Fact]
    public void Then_the_Argentine_code_is_kept_Run()
    {
        var phoneNumber = PhoneNumber.Create("+54 9 11 2233-4455", TestData.SpainCallingCode);

        phoneNumber.Value!.Value.ShouldBe("+5491122334455");
    }
}
