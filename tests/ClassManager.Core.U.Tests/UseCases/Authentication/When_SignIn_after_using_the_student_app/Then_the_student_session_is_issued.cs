using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignIn_after_using_the_student_app;

public sealed class Then_the_student_session_is_issued
{
    [Fact]
    public async Task Then_the_student_session_is_issued_Run()
    {
        var builder = new SignInUseCaseBuilder();
        builder.WithLastSessionIn(builder.Student.BusinessId, AccountKinds.Student);
        builder.StudentApp
            .Setup(directory => directory.FindAsync(builder.UserId, builder.Student.BusinessId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(builder.Student);

        await builder.Build().ExecuteAsync(SignInUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.Tokens.Verify(
            service => service.IssueAsync(
                new SessionUser(builder.UserId, TestData.OwnerEmail, builder.Student.BusinessId, AccountKinds.StudentRoleName, AccountKinds.Student),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
