using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Roles;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_BusinessMember_gets_a_custom_role_with_own_permissions_without_instructor;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var role = CustomRole.Create("Coach que cobra", [Permissions.Sessions.ViewOwn, Permissions.Payments.Record], null, TestData.Now).Value!;

        var member = BusinessMember.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), MemberRole.Custom(role), instructorId: null);

        member.Error!.Code.ShouldBe(MemberErrorCodes.InstructorRequired);
    }
}
