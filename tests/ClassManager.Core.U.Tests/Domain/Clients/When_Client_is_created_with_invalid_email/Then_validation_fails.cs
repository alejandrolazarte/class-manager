using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_Client_is_created_with_invalid_email;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var client = Client.Create(TestData.ClientFullName, TestData.PhoneNumber(), "ana-at-example.com", null, TestData.Now);

        client.Error!.FieldName.ShouldBe(nameof(Client.Email));
    }
}
