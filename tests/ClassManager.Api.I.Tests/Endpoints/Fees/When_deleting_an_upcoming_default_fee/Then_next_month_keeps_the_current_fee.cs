namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_deleting_an_upcoming_default_fee;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_next_month_keeps_the_current_fee(ApiFixture fixture)
{
    [Fact]
    public async Task Then_next_month_keeps_the_current_fee_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync();
        await business.HttpClient.EnrollClientAsync();
        await business.HttpClient.SetDefaultFeeAsync(15000m, FeeRequests.NextMonth);

        using var response = await business.HttpClient.DeleteDefaultFeeChangeAsync(FeeRequests.NextMonth);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await business.HttpClient.GetMonthlyFeesAsync(FeeRequests.NextMonth))!.Clients.Single().Fee.ShouldBe(FeeRequests.DefaultFee);
    }
}
