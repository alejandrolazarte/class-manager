namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_a_branch_connects_again_after_stopping;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_gets_its_stripe_account_back(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_gets_its_stripe_account_back_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        await brand.HttpClient.StartOnlinePaymentOnboardingAsync();
        using var stopResponse = await brand.HttpClient.DeleteOnlinePaymentAccountAsync();

        await brand.HttpClient.StartOnlinePaymentOnboardingAsync();

        fixture.ApiFactory.OnlinePaymentGateway.AccountsCreatedFor(brand.BusinessId).Count.ShouldBe(1);
    }
}
