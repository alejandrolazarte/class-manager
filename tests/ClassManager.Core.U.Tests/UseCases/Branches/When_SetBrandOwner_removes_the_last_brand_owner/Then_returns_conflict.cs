using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Organizations;
using ClassManager.Core.UseCases.Branches;

namespace ClassManager.Core.U.Tests.UseCases.Branches.When_SetBrandOwner_removes_the_last_brand_owner;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var business = TestData.Business();
        var member = BusinessMember.CreateBranchOwner(business.Id, Guid.CreateVersion7());
        var businesses = new Mock<IBusinessRepository>();
        businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(business);
        var businessMembers = new Mock<IBusinessMemberRepository>();
        businessMembers.Setup(repository => repository.GetForUpdateAsync(member.Id, It.IsAny<CancellationToken>())).ReturnsAsync(member);
        var organizationMembers = new Mock<IOrganizationMemberRepository>();
        organizationMembers
            .Setup(repository => repository.FindForUpdateAsync(business.OrganizationId, member.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(OrganizationMember.CreateBrandOwner(business.OrganizationId, member.UserId));
        organizationMembers
            .Setup(repository => repository.ListBrandOwnerUserIdsAsync(business.OrganizationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([member.UserId]);
        var currentMember = new Mock<ICurrentMember>();
        currentMember
            .Setup(current => current.GetAccessAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemberAccess(Guid.CreateVersion7(), business.Id, null, null, IsBrandOwner: true, SystemRolePermissions.BrandOwner));
        var useCase = new SetBrandOwnerUseCase(
            businesses.Object, businessMembers.Object, organizationMembers.Object, currentMember.Object, new Mock<IUnitOfWork>().Object);

        var response = await useCase.ExecuteAsync(new SetBrandOwnerCommand(member.Id, IsBrandOwner: false), CancellationToken.None);

        response.Error!.Code.ShouldBe(BranchErrorCodes.LastBrandOwner);
        organizationMembers.Verify(repository => repository.Remove(It.IsAny<OrganizationMember>()), Times.Never);
    }
}
