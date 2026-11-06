namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_the_brand_owner_opens_online_payments_in_another_branch;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_brand_accounts_are_offered(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_brand_accounts_are_offered_Run()
    {
        var brand = await fixture.SignUpBrandWithTwoBranchesAsync();
        await brand.Brand.HttpClient.StartOnlinePaymentOnboardingAsync();
        var account = await fixture.OnlinePaymentAccountOfAsync(brand.Brand.BusinessId);

        var onlinePayments = await brand.SecondBranch.ReadOnlinePaymentsAsync();

        onlinePayments.BrandAccounts.Select(brandAccount => brandAccount.Id).ShouldBe([account.Id]);
    }
}
