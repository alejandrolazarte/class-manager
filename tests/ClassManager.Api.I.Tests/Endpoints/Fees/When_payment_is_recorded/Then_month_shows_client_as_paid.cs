using ClassManager.Core.Domain.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_payment_is_recorded;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_month_shows_client_as_paid(ApiFixture fixture)
{
    [Fact]
    public async Task Then_month_shows_client_as_paid_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.SetDefaultFeeAsync();
        var clientId = await business.HttpClient.EnrollClientAsync();

        using var response = await business.HttpClient.PostPaymentAsync(clientId);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var fees = await business.HttpClient.GetMonthlyFeesAsync();
        fees!.Clients.Single().Status.ShouldBe(FeeStatus.Paid);
        fees.TotalPaid.ShouldBe(FeeRequests.DefaultFee);
    }
}
