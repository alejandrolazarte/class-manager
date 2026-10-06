using ClassManager.Core.Domain.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_a_deleted_upcoming_fee_is_scheduled_again;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_new_fee_applies(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_new_fee_applies_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();
        using var firstSchedule = await business.HttpClient.PutBillingPlanAsync(clientId, BillingPlanKind.CustomFee, 9000m, FeeRequests.NextMonth);
        using var deletion = await business.HttpClient.DeleteBillingPlanChangeAsync(clientId, FeeRequests.NextMonth);
        deletion.EnsureSuccessStatusCode();

        using var response = await business.HttpClient.PutBillingPlanAsync(clientId, BillingPlanKind.CustomFee, 7000m, FeeRequests.NextMonth);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await business.HttpClient.GetMonthlyFeesAsync(FeeRequests.NextMonth))!.Clients.Single().Fee.ShouldBe(7000m);
    }
}
