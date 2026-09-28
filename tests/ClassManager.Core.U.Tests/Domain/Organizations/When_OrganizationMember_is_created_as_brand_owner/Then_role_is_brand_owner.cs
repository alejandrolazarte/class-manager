using ClassManager.Core.Domain.Organizations;

namespace ClassManager.Core.U.Tests.Domain.Organizations.When_OrganizationMember_is_created_as_brand_owner;

public sealed class Then_role_is_brand_owner
{
    [Fact]
    public void Then_role_is_brand_owner_Run()
    {
        var organizationId = Guid.CreateVersion7();
        var userId = Guid.CreateVersion7();

        var member = OrganizationMember.CreateBrandOwner(organizationId, userId);

        member.Role.ShouldBe(OrganizationRole.BrandOwner);
        member.OrganizationId.ShouldBe(organizationId);
        member.UserId.ShouldBe(userId);
    }
}
