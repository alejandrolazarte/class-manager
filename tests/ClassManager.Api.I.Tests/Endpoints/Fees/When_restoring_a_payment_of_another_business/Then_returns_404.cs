using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_restoring_a_payment_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();
        using var paymentResponse = await business.HttpClient.PostPaymentAsync(clientId);
        var payment = await paymentResponse.Content.ReadFromJsonAsync<PaymentResponse>(ApiRequests.JsonOptions);
        (await business.HttpClient.DeletePaymentAsync(payment!.Id)).EnsureSuccessStatusCode();

        using var response = await otherBusiness.HttpClient.RestorePaymentAsync(payment.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
