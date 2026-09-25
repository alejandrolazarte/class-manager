using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignIn_with_unknown_email;

public sealed class Then_returns_same_invalid_credentials
{
    [Fact]
    public async Task Then_returns_same_invalid_credentials_Run()
    {
        const string unknownEmail = "nobody@example.com";
        const string wrongPassword = "not the right passphrase";
        var builder = new SignInUseCaseBuilder();
        builder.Identity
            .Setup(service => service.VerifyCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CredentialVerification.InvalidCredentials);
        var useCase = builder.Build();
        var wrongPasswordResponse = await useCase.ExecuteAsync(
            SignInUseCaseBuilder.ValidCommand() with { Password = wrongPassword }, CancellationToken.None);

        var unknownEmailResponse = await useCase.ExecuteAsync(
            SignInUseCaseBuilder.ValidCommand() with { Email = unknownEmail }, CancellationToken.None);

        (unknownEmailResponse.Error!.Code, unknownEmailResponse.Error.Message)
            .ShouldBe((wrongPasswordResponse.Error!.Code, wrongPasswordResponse.Error.Message));
        unknownEmailResponse.Error.Code.ShouldBe(AuthenticationErrorCodes.InvalidCredentials);
    }
}
