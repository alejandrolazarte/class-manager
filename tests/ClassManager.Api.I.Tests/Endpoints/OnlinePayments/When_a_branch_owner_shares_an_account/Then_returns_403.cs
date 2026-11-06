using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_a_branch_owner_shares_an_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var brand = await fixture.SignUpBrandWithTwoBranchesAsync();
        await brand.Brand.HttpClient.StartOnlinePaymentOnboardingAsync();
        var account = await fixture.OnlinePaymentAccountOfAsync(brand.Brand.BusinessId);
        using var branchOwner = await fixture.SeedMemberAsync(brand.SecondBusinessId, BusinessRole.BranchOwner);

        using var response = await branchOwner.PutOnlinePaymentAccountAsync(account.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
