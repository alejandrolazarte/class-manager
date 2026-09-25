using ClassManager.Core.Domain.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_setting_default_fee;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_enrolled_clients_owe_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_enrolled_clients_owe_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();

        await business.HttpClient.SetDefaultFeeAsync();

        var fees = await business.HttpClient.GetMonthlyFeesAsync();
        var client = fees!.Clients.Single();
        client.ClientId.ShouldBe(clientId);
        client.Status.ShouldBe(FeeStatus.Unpaid);
        client.Balance.ShouldBe(FeeRequests.DefaultFee);
    }
}
