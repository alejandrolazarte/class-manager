using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_Client_is_created_with_valid_data;

public sealed class Then_name_is_trimmed
{
    [Fact]
    public void Then_name_is_trimmed_Run()
    {
        var client = Client.Create("  Ana Pérez  ", TestData.PhoneNumber(), "ana@example.com", "Color 6.1 + 20 vol", TestData.Now);

        client.Value!.FullName.ShouldBe(TestData.ClientFullName);
    }
}
