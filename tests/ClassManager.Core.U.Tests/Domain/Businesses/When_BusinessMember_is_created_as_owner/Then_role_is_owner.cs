using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_BusinessMember_is_created_as_owner;

public sealed class Then_role_is_owner
{
    [Fact]
    public void Then_role_is_owner_Run()
    {
        var businessId = Guid.CreateVersion7();
        var userId = Guid.CreateVersion7();

        var member = BusinessMember.CreateOwner(businessId, userId);

        member.Role.ShouldBe(BusinessRole.Owner);
        member.TenantId.ShouldBe(businessId);
        member.UserId.ShouldBe(userId);
    }
}
