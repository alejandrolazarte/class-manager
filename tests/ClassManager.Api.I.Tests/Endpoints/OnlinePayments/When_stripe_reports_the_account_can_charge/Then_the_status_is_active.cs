using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_stripe_reports_the_account_can_charge;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_status_is_active(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_status_is_active_Run()
    {
        var brand = await fixture.SignUpBrandWithOnlinePaymentsAsync();
        await brand.HttpClient.StartOnlinePaymentOnboardingAsync();
        var account = await fixture.OnlinePaymentAccountOfAsync(brand.BusinessId);
        fixture.ApiFactory.OnlinePaymentGateway.Set(account.ExternalAccountId, FakeOnlinePaymentGateway.ActiveAccountState);

        using var response = await fixture.PostStripeConnectEventAsync(OnlinePaymentRequests.AccountUpdated, account.ExternalAccountId);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await brand.HttpClient.ReadOnlinePaymentsAsync()).Status.ShouldBe(OnlinePaymentStatus.Active);
    }
}
