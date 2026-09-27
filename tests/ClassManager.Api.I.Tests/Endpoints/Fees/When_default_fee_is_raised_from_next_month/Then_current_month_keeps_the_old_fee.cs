namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_default_fee_is_raised_from_next_month;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_current_month_keeps_the_old_fee(ApiFixture fixture)
{
    [Fact]
    public async Task Then_current_month_keeps_the_old_fee_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync();
        await business.HttpClient.EnrollClientAsync();

        var settings = await business.HttpClient.SetDefaultFeeAsync(15000m, FeeRequests.NextMonth);

        (await business.HttpClient.GetMonthlyFeesAsync())!.Clients.Single().Fee.ShouldBe(FeeRequests.DefaultFee);
        (await business.HttpClient.GetMonthlyFeesAsync(FeeRequests.NextMonth))!.Clients.Single().Fee.ShouldBe(15000m);
        settings.DefaultMonthlyFee.ShouldBe(FeeRequests.DefaultFee);
        settings.DefaultMonthlyFeeChanges.Select(change => change.EffectiveFrom).ShouldBe([FeeRequests.CurrentMonth, FeeRequests.NextMonth]);
    }
}
