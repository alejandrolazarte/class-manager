using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_Client_is_created_with_short_name;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var client = Client.Create(" A ", TestData.PhoneNumber(), null, null, TestData.Now);

        client.Error!.Kind.ShouldBe(ErrorKind.Validation);
        client.Error.FieldName.ShouldBe(nameof(Client.FullName));
    }
}
