using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_a_branch_owner_reads_online_payments;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_other_branches_accounts_are_not_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_other_branches_accounts_are_not_listed_Run()
    {
        var brand = await fixture.SignUpBrandWithTwoBranchesAsync();
        await brand.Brand.HttpClient.StartOnlinePaymentOnboardingAsync();
        using var branchOwner = await fixture.SeedMemberAsync(brand.SecondBusinessId, BusinessRole.BranchOwner);

        var onlinePayments = await branchOwner.ReadOnlinePaymentsAsync();

        onlinePayments.BrandAccounts.ShouldBeEmpty();
    }
}
