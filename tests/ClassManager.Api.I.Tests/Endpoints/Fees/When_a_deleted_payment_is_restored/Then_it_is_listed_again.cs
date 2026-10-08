using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_a_deleted_payment_is_restored;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_listed_again(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_listed_again_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();
        using var paymentResponse = await business.HttpClient.PostPaymentAsync(clientId);
        var payment = await paymentResponse.Content.ReadFromJsonAsync<PaymentResponse>(ApiRequests.JsonOptions);
        (await business.HttpClient.DeletePaymentAsync(payment!.Id)).EnsureSuccessStatusCode();

        using var response = await business.HttpClient.RestorePaymentAsync(payment.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await business.HttpClient.GetClientPaymentsAsync(clientId))!.Single().Id.ShouldBe(payment.Id);
    }
}
