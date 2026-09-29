using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_BusinessMember_is_created_as_branch_owner_without_instructor;

public sealed class Then_member_is_created
{
    [Fact]
    public void Then_member_is_created_Run()
    {
        var member = BusinessMember.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), BusinessRole.BranchOwner, instructorId: null);

        member.Value!.Role.ShouldBe(BusinessRole.BranchOwner);
    }
}
