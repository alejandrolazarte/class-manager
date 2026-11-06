namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_the_branch_continues_the_onboarding;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_same_stripe_account_is_used(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_same_stripe_account_is_used_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        await brand.HttpClient.StartOnlinePaymentOnboardingAsync();

        await brand.HttpClient.StartOnlinePaymentOnboardingAsync();

        fixture.ApiFactory.OnlinePaymentGateway.AccountsCreatedFor(brand.BusinessId).Count.ShouldBe(1);
    }
}
