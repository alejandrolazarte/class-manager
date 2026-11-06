namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_a_branch_connects_online_payments;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_the_stripe_onboarding_link(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_the_stripe_onboarding_link_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);

        var onboardingUrl = await brand.HttpClient.StartOnlinePaymentOnboardingAsync();

        var account = await fixture.OnlinePaymentAccountOfAsync(brand.BusinessId);
        onboardingUrl.ShouldBe(FakeOnlinePaymentGateway.OnboardingUrlOf(account.ExternalAccountId));
    }
}
