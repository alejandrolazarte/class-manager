using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Businesses.When_BusinessMember_is_created_as_coach_without_instructor;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var member = BusinessMember.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), BusinessRole.Coach, instructorId: null);

        member.Error!.Code.ShouldBe(MemberErrorCodes.InstructorRequired);
    }
}
