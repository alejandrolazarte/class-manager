using ClassManager.Core.Domain.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_deleting_an_upcoming_client_fee;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_next_month_keeps_the_current_fee(ApiFixture fixture)
{
    [Fact]
    public async Task Then_next_month_keeps_the_current_fee_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();
        using var scheduled = await business.HttpClient.PutBillingPlanAsync(clientId, BillingPlanKind.CustomFee, 9000m, FeeRequests.NextMonth);
        scheduled.EnsureSuccessStatusCode();

        using var response = await business.HttpClient.DeleteBillingPlanChangeAsync(clientId, FeeRequests.NextMonth);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await business.HttpClient.GetMonthlyFeesAsync(FeeRequests.NextMonth))!.Clients.Single().Fee.ShouldBe(FeeRequests.DefaultFee);
    }
}
