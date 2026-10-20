using ClassManager.Core.Domain.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_the_business_charges_with_class_packs;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_clients_without_plan_pay_per_class(ApiFixture fixture)
{
    [Fact]
    public async Task Then_clients_without_plan_pay_per_class_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();

        var businessResponse = await business.HttpClient.SetDefaultFeeAsync(amount: null, billingMode: BusinessBillingMode.ClassPacks);

        businessResponse.BillingMode.ShouldBe(BusinessBillingMode.ClassPacks);
        var fees = await business.HttpClient.GetMonthlyFeesAsync();
        fees!.BillingMode.ShouldBe(BusinessBillingMode.ClassPacks);
        fees.Clients.ShouldBeEmpty();
        fees.ClassPackClients.Single().ClientId.ShouldBe(clientId);
    }
}
