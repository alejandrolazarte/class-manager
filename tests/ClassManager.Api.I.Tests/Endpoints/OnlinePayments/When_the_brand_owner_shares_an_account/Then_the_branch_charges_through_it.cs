namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_the_brand_owner_shares_an_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_branch_charges_through_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_branch_charges_through_it_Run()
    {
        var brand = await fixture.SignUpBrandWithTwoBranchesAsync();
        await brand.Brand.HttpClient.StartOnlinePaymentOnboardingAsync();
        var account = await fixture.OnlinePaymentAccountOfAsync(brand.Brand.BusinessId);

        using var response = await brand.SecondBranch.PutOnlinePaymentAccountAsync(account.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await fixture.OnlinePaymentAccountOfAsync(brand.SecondBusinessId)).Id.ShouldBe(account.Id);
    }
}
