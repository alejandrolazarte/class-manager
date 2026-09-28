using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Services.AccessScopes.When_member_can_view_every_student;

public sealed class Then_client_scope_is_unrestricted
{
    [Fact]
    public async Task Then_client_scope_is_unrestricted_Run()
    {
        var scopes = new ClassManager.Core.Services.AccessScopes(new CurrentMemberFake(BusinessRole.BranchOwner, instructorId: null));

        var scope = await scopes.ForClientsAsync(CancellationToken.None);

        scope.ShouldBeNull();
    }
}
