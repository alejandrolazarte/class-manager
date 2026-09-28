using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_BusinessMember_is_created_as_branch_owner;

public sealed class Then_role_is_branch_owner
{
    [Fact]
    public void Then_role_is_branch_owner_Run()
    {
        var businessId = Guid.CreateVersion7();
        var userId = Guid.CreateVersion7();

        var member = BusinessMember.CreateBranchOwner(businessId, userId);

        member.Role.ShouldBe(BusinessRole.BranchOwner);
        member.TenantId.ShouldBe(businessId);
        member.UserId.ShouldBe(userId);
    }
}
