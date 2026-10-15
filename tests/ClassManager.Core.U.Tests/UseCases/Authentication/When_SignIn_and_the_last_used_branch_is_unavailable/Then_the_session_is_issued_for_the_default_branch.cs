using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignIn_and_the_last_used_branch_is_unavailable;

public sealed class Then_the_session_is_issued_for_the_default_branch
{
    [Fact]
    public async Task Then_the_session_is_issued_for_the_default_branch_Run()
    {
        var builder = new SignInUseCaseBuilder();
        builder.WithLastSessionIn(builder.LastUsedBranch.BusinessId, AccountKinds.Team);

        await builder.Build().ExecuteAsync(SignInUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.Tokens.Verify(
            service => service.IssueAsync(
                It.Is<SessionUser>(user => user.TenantId == builder.Branch.BusinessId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
