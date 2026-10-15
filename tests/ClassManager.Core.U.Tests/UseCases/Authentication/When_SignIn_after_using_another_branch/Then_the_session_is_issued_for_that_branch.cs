using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignIn_after_using_another_branch;

public sealed class Then_the_session_is_issued_for_that_branch
{
    [Fact]
    public async Task Then_the_session_is_issued_for_that_branch_Run()
    {
        var builder = new SignInUseCaseBuilder();
        builder.WithLastSessionIn(builder.LastUsedBranch.BusinessId, AccountKinds.Team);
        builder.WithAccessToLastUsedBranch();

        await builder.Build().ExecuteAsync(SignInUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.Tokens.Verify(
            service => service.IssueAsync(
                It.Is<SessionUser>(user => user.TenantId == builder.LastUsedBranch.BusinessId && user.Kind == AccountKinds.Team),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
