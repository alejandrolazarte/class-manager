using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_the_branch_returns_from_the_onboarding;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_status_is_read_from_stripe(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_status_is_read_from_stripe_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        await brand.HttpClient.StartOnlinePaymentOnboardingAsync();
        var account = await fixture.OnlinePaymentAccountOfAsync(brand.BusinessId);
        fixture.ApiFactory.OnlinePaymentGateway.Set(account.ExternalAccountId, new OnlinePaymentAccountState(DetailsSubmitted: true, ChargesEnabled: false, PayoutsEnabled: false));

        using var response = await brand.HttpClient.PostOnlinePaymentRefreshAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await brand.HttpClient.ReadOnlinePaymentsAsync()).Status.ShouldBe(OnlinePaymentStatus.Restricted);
    }
}
