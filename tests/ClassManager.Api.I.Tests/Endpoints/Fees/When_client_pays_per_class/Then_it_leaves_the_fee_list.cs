using ClassManager.Core.Domain.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_client_pays_per_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_leaves_the_fee_list(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_leaves_the_fee_list_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();

        using var response = await business.HttpClient.PutBillingPlanAsync(clientId, BillingPlanKind.ClassPacks);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var fees = await business.HttpClient.GetMonthlyFeesAsync();
        fees!.Clients.ShouldBeEmpty();
        fees.ClassPackClients.Single().ClientId.ShouldBe(clientId);
    }
}
