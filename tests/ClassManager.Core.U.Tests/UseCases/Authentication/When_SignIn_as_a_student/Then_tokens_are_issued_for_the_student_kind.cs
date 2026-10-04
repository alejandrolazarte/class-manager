using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignIn_as_a_student;

public sealed class Then_tokens_are_issued_for_the_student_kind
{
    [Fact]
    public async Task Then_tokens_are_issued_for_the_student_kind_Run()
    {
        var builder = new SignInUseCaseBuilder();
        builder.WithoutBranch();
        builder.WithStudentAccount();

        var response = await builder.Build().ExecuteAsync(SignInUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        builder.Tokens.Verify(
            service => service.IssueAsync(
                new SessionUser(builder.UserId, TestData.OwnerEmail, builder.Student.BusinessId, AccountKinds.StudentRoleName, AccountKinds.Student),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
