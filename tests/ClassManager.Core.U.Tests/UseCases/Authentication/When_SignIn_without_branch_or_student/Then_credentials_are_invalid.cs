using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignIn_without_branch_or_student;

public sealed class Then_credentials_are_invalid
{
    [Fact]
    public async Task Then_credentials_are_invalid_Run()
    {
        var builder = new SignInUseCaseBuilder();
        builder.WithoutBranch();

        var response = await builder.Build().ExecuteAsync(SignInUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(AuthenticationErrorCodes.InvalidCredentials);
    }
}
