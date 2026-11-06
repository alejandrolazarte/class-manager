namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_a_branch_stops_using_a_shared_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_other_branch_keeps_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_other_branch_keeps_it_Run()
    {
        var brand = await fixture.SignUpBrandWithTwoBranchesAsync();
        await brand.Brand.HttpClient.StartOnlinePaymentOnboardingAsync();
        var account = await fixture.OnlinePaymentAccountOfAsync(brand.Brand.BusinessId);
        using var shareResponse = await brand.SecondBranch.PutOnlinePaymentAccountAsync(account.Id);

        using var response = await brand.SecondBranch.DeleteOnlinePaymentAccountAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await fixture.OnlinePaymentAccountOfAsync(brand.Brand.BusinessId)).Id.ShouldBe(account.Id);
    }
}
