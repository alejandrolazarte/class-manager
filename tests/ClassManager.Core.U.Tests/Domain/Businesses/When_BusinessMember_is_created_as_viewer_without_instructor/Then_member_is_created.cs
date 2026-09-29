using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_BusinessMember_is_created_as_viewer_without_instructor;

public sealed class Then_member_is_created
{
    [Fact]
    public void Then_member_is_created_Run()
    {
        var member = BusinessMember.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), BusinessRole.Viewer, instructorId: null);

        member.Value!.Role.ShouldBe(BusinessRole.Viewer);
    }
}
