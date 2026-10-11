namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_business_currency_changes_after_a_payment;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_earlier_payment_keeps_its_currency(ApiFixture fixture)
{
    private const string DollarCurrencyCode = "USD";
    private const decimal DollarAmount = 55m;

    [Fact]
    public async Task Then_the_earlier_payment_keeps_its_currency_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();
        (await business.HttpClient.PostPaymentAsync(clientId)).EnsureSuccessStatusCode();
        (await business.HttpClient.PutBusinessAsync(SettingsRequests.SettingsOf(business, currencyCode: DollarCurrencyCode))).EnsureSuccessStatusCode();

        (await business.HttpClient.PostPaymentAsync(clientId, DollarAmount)).EnsureSuccessStatusCode();

        var payments = await business.HttpClient.GetClientPaymentsAsync(clientId);
        payments.ShouldNotBeNull();
        payments.Single(payment => payment.Amount == FeeRequests.DefaultFee).CurrencyCode.ShouldBe(ApiFixture.CurrencyCode);
        payments.Single(payment => payment.Amount == DollarAmount).CurrencyCode.ShouldBe(DollarCurrencyCode);
    }
}
