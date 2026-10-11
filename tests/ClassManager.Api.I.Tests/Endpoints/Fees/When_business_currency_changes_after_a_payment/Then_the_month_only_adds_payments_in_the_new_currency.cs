namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_business_currency_changes_after_a_payment;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_month_only_adds_payments_in_the_new_currency(ApiFixture fixture)
{
    private const string DollarCurrencyCode = "USD";
    private const decimal DollarAmount = 55m;

    [Fact]
    public async Task Then_the_month_only_adds_payments_in_the_new_currency_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();
        (await business.HttpClient.PostPaymentAsync(clientId)).EnsureSuccessStatusCode();
        (await business.HttpClient.PutBusinessAsync(SettingsRequests.SettingsOf(business, currencyCode: DollarCurrencyCode))).EnsureSuccessStatusCode();

        (await business.HttpClient.PostPaymentAsync(clientId, DollarAmount)).EnsureSuccessStatusCode();

        var fees = await business.HttpClient.GetMonthlyFeesAsync();
        fees!.TotalPaid.ShouldBe(DollarAmount);
    }
}
