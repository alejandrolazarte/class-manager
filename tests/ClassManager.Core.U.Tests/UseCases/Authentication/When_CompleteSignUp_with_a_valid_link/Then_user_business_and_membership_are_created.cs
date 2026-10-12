using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_CompleteSignUp_with_a_valid_link;

public sealed class Then_user_business_and_membership_are_created
{
    [Fact]
    public async Task Then_user_business_and_membership_are_created_Run()
    {
        var builder = new CompleteSignUpUseCaseBuilder();

        var response = await builder.Build().ExecuteAsync(CompleteSignUpUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        builder.Identity.Verify(
            service => service.CreateOwnerAsync(It.Is<OwnerAccount>(account => account.Email == TestData.OwnerEmail), It.IsAny<CancellationToken>()),
            Times.Once);
        builder.AddedBusiness!.Name.ShouldBe(TestData.BusinessName);
        builder.AddedMember.ShouldBe(BusinessMember.CreateBranchOwner(builder.AddedBusiness.Id, builder.UserId), new BusinessMemberLinkComparer());
        builder.Transaction.Verify(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private sealed class BusinessMemberLinkComparer : IEqualityComparer<BusinessMember?>
    {
        public bool Equals(BusinessMember? x, BusinessMember? y) =>
            x?.TenantId == y?.TenantId && x?.UserId == y?.UserId && x?.Role == y?.Role;

        public int GetHashCode(BusinessMember? obj) => HashCode.Combine(obj?.TenantId, obj?.UserId, obj?.Role);
    }
}
