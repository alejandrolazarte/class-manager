using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignIn_as_a_team_member_and_a_student;

public sealed class Then_the_team_session_is_issued
{
    [Fact]
    public async Task Then_the_team_session_is_issued_Run()
    {
        var builder = new SignInUseCaseBuilder();
        builder.WithStudentAccount();

        await builder.Build().ExecuteAsync(SignInUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.Tokens.Verify(
            service => service.IssueAsync(It.Is<SessionUser>(user => user.Kind == AccountKinds.Team), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
