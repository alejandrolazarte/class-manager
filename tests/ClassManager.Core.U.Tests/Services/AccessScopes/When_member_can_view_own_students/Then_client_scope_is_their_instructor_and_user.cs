using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Services.AccessScopes.When_member_can_view_own_students;

public sealed class Then_client_scope_is_their_instructor_and_user
{
    [Fact]
    public async Task Then_client_scope_is_their_instructor_and_user_Run()
    {
        var instructorId = Guid.CreateVersion7();
        var currentMember = new CurrentMemberFake(BusinessRole.Coach, instructorId);
        var scopes = new ClassManager.Core.Services.AccessScopes(currentMember);

        var scope = await scopes.ForClientsAsync(CancellationToken.None);

        scope.ShouldBe(new ClientScope(instructorId, currentMember.UserId));
    }
}
