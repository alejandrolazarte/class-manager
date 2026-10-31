using ClassManager.Core.Domain.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_the_business_first_chooses_how_it_charges_with_clients_already_registered;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_keep_it_after_a_later_switch(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_keep_it_after_a_later_switch_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();
        await business.HttpClient.SetDefaultFeeAsync(amount: null, billingMode: BusinessBillingMode.ClassPacks);

        await business.HttpClient.SetDefaultFeeAsync(amount: 25000m, billingMode: BusinessBillingMode.MonthlyFee);

        var fees = await business.HttpClient.GetMonthlyFeesAsync();
        fees!.Clients.ShouldBeEmpty();
        fees.ClassPackClients.Single().ClientId.ShouldBe(clientId);
    }
}
