using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Services.AccessScopes.When_member_has_every_instructor_permission;

public sealed class Then_every_instructor_is_included
{
    [Fact]
    public async Task Then_every_instructor_is_included_Run()
    {
        var scopes = new ClassManager.Core.Services.AccessScopes(new CurrentMemberFake(BusinessRole.Viewer, instructorId: null));

        var scope = await scopes.ForInstructorsAsync(Permissions.Sessions.ViewAll, CancellationToken.None);

        scope.Includes(Guid.CreateVersion7()).ShouldBeTrue();
    }
}
