using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_Client_is_updated_with_short_name;

public sealed class Then_validation_fails_and_details_are_kept
{
    [Fact]
    public void Then_validation_fails_and_details_are_kept_Run()
    {
        var client = TestData.Client();

        var update = client.Update(" A ", TestData.PhoneNumber(), "ana@example.com", null);

        update.Error!.FieldName.ShouldBe(nameof(Client.FullName));
        client.ShouldSatisfyAllConditions(
            () => client.FullName.ShouldBe(TestData.ClientFullName),
            () => client.Email.ShouldBeNull());
    }
}
