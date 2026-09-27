using ClassManager.Core.Domain.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_setting_client_fee;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_overrides_the_default(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_overrides_the_default_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();

        using var response = await business.HttpClient.PutBillingPlanAsync(clientId, BillingPlanKind.CustomFee, 9000m);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await business.HttpClient.GetMonthlyFeesAsync())!.Clients.Single().Fee.ShouldBe(9000m);
    }
}
