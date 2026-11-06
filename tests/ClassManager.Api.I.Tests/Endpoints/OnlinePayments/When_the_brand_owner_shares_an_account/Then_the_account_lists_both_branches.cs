namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_the_brand_owner_shares_an_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_account_lists_both_branches(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_account_lists_both_branches_Run()
    {
        var brand = await fixture.SignUpBrandWithTwoBranchesAsync();
        await brand.Brand.HttpClient.StartOnlinePaymentOnboardingAsync();
        var account = await fixture.OnlinePaymentAccountOfAsync(brand.Brand.BusinessId);
        using var shareResponse = await brand.SecondBranch.PutOnlinePaymentAccountAsync(account.Id);

        var onlinePayments = await brand.Brand.HttpClient.ReadOnlinePaymentsAsync();

        onlinePayments.Account!.BranchNames.ShouldContain(brand.SecondBranchName);
    }
}
