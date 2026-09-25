using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignIn_with_wrong_password;

public sealed class Then_returns_invalid_credentials
{
    [Fact]
    public async Task Then_returns_invalid_credentials_Run()
    {
        const string wrongPassword = "not the right passphrase";
        var builder = new SignInUseCaseBuilder();
        builder.Identity
            .Setup(service => service.VerifyCredentialsAsync(TestData.OwnerEmail, wrongPassword, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CredentialVerification.InvalidCredentials);

        var response = await builder.Build().ExecuteAsync(SignInUseCaseBuilder.ValidCommand() with { Password = wrongPassword }, CancellationToken.None);

        response.Error!.Code.ShouldBe(AuthenticationErrorCodes.InvalidCredentials);
        response.Error.Kind.ShouldBe(ErrorKind.Unauthorized);
    }
}
