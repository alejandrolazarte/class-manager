using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignIn_while_locked_out;

public sealed class Then_returns_locked_out
{
    [Fact]
    public async Task Then_returns_locked_out_Run()
    {
        var builder = new SignInUseCaseBuilder();
        builder.Identity
            .Setup(service => service.VerifyCredentialsAsync(TestData.OwnerEmail, TestData.OwnerPassword, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CredentialVerification.LockedOut);

        var response = await builder.Build().ExecuteAsync(SignInUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(AuthenticationErrorCodes.LockedOut);
        response.Error.Kind.ShouldBe(ErrorKind.Locked);
    }
}
