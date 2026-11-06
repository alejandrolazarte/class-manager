namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_another_brand_reads_online_payments;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_this_brand_accounts_are_not_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_this_brand_accounts_are_not_listed_Run()
    {
        var owner = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        await owner.HttpClient.StartOnlinePaymentOnboardingAsync();
        var stranger = await fixture.SignUpBrandWithTwoBranchesAsync();

        var onlinePayments = await stranger.SecondBranch.ReadOnlinePaymentsAsync();

        onlinePayments.BrandAccounts.ShouldBeEmpty();
    }
}
