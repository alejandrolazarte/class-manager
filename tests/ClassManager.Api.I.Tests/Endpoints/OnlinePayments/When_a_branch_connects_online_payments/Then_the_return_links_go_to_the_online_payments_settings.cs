namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_a_branch_connects_online_payments;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_return_links_go_to_the_online_payments_settings(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_return_links_go_to_the_online_payments_settings_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);

        await brand.HttpClient.StartOnlinePaymentOnboardingAsync();

        var account = await fixture.OnlinePaymentAccountOfAsync(brand.BusinessId);
        var link = fixture.ApiFactory.OnlinePaymentGateway.OnboardingLinksFor(account.ExternalAccountId).Single();
        new[] { link.ReturnUrl.ToString(), link.RefreshUrl.ToString() }
            .ShouldBe([OnlinePaymentRequests.OnboardingReturnedUrl, OnlinePaymentRequests.OnboardingExpiredUrl]);
    }
}
