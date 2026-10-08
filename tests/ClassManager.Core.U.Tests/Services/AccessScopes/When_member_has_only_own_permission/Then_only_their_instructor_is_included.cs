using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Services.AccessScopes.When_member_has_only_own_permission;

public sealed class Then_only_their_instructor_is_included
{
    [Fact]
    public async Task Then_only_their_instructor_is_included_Run()
    {
        var instructorId = Guid.CreateVersion7();
        var scopes = new ClassManager.Core.Services.AccessScopes(new CurrentMemberFake(BusinessRole.Instructor, instructorId));

        var scope = await scopes.ForInstructorsAsync(Permissions.Sessions.ViewAll, CancellationToken.None);

        scope.Includes(instructorId).ShouldBeTrue();
        scope.Includes(Guid.CreateVersion7()).ShouldBeFalse();
    }
}
