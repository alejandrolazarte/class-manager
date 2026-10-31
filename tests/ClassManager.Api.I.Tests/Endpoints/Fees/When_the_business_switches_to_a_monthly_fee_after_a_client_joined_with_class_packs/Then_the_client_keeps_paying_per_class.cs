using ClassManager.Core.Domain.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_the_business_switches_to_a_monthly_fee_after_a_client_joined_with_class_packs;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_client_keeps_paying_per_class(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_client_keeps_paying_per_class_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync(amount: null, billingMode: BusinessBillingMode.ClassPacks);
        var clientId = await business.HttpClient.EnrollClientAsync();

        await business.HttpClient.SetDefaultFeeAsync(amount: 25000m, billingMode: BusinessBillingMode.MonthlyFee);

        var fees = await business.HttpClient.GetMonthlyFeesAsync();
        fees!.BillingMode.ShouldBe(BusinessBillingMode.MonthlyFee);
        fees.Clients.ShouldBeEmpty();
        fees.ClassPackClients.Single().ClientId.ShouldBe(clientId);
    }
}
