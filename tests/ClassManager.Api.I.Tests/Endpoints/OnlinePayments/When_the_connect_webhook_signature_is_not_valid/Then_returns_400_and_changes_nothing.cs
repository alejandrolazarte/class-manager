using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_the_connect_webhook_signature_is_not_valid;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400_and_changes_nothing(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_and_changes_nothing_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        await brand.HttpClient.StartOnlinePaymentOnboardingAsync();
        var account = await fixture.OnlinePaymentAccountOfAsync(brand.BusinessId);
        fixture.ApiFactory.OnlinePaymentGateway.Set(account.ExternalAccountId, FakeOnlinePaymentGateway.ActiveAccountState);

        using var response = await fixture.PostStripeConnectEventAsync(
            OnlinePaymentRequests.AccountUpdated,
            account.ExternalAccountId,
            BusinessApiFactory.TestStripeWebhookSecret);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await brand.HttpClient.ReadOnlinePaymentsAsync()).Status.ShouldBe(OnlinePaymentStatus.Pending);
    }
}
