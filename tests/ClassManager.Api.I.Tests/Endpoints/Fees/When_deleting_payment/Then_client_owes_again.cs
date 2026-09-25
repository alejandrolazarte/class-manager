using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_deleting_payment;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_client_owes_again(ApiFixture fixture)
{
    [Fact]
    public async Task Then_client_owes_again_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();
        using var paymentResponse = await business.HttpClient.PostPaymentAsync(clientId);
        var payment = await paymentResponse.Content.ReadFromJsonAsync<PaymentResponse>(ApiRequests.JsonOptions);

        using var response = await business.HttpClient.DeleteAsync(new Uri($"{ApiRoutes.Payments}/{payment!.Id}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await business.HttpClient.GetMonthlyFeesAsync())!.Clients.Single().Status.ShouldBe(FeeStatus.Unpaid);
    }
}
