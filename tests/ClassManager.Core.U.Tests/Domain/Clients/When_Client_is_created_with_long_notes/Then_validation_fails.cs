using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_Client_is_created_with_long_notes;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var notes = new string('x', Client.NotesMaxLength + 1);

        var client = Client.Create(TestData.ClientFullName, TestData.PhoneNumber(), null, notes, TestData.Now);

        client.Error!.FieldName.ShouldBe(nameof(Client.Notes));
    }
}
