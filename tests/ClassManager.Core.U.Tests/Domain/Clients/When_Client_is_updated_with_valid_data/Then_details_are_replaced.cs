using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_Client_is_updated_with_valid_data;

public sealed class Then_details_are_replaced
{
    [Fact]
    public void Then_details_are_replaced_Run()
    {
        var client = TestData.Client();
        var newPhoneNumber = PhoneNumber.Create("11 5566-7788", TestData.DefaultCountryCallingCode).Value!;

        var update = client.Update(" Ana María Pérez ", newPhoneNumber, " ana@example.com ", " Allergic to chlorine ");

        update.IsSuccess.ShouldBeTrue();
        client.ShouldSatisfyAllConditions(
            () => client.FullName.ShouldBe("Ana María Pérez"),
            () => client.PhoneNumber.ShouldBe(newPhoneNumber),
            () => client.Email.ShouldBe("ana@example.com"),
            () => client.Notes.ShouldBe("Allergic to chlorine"));
    }
}
