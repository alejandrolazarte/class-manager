using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_CreatePasswordResetToken_for_unknown_email;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        const string unknownEmail = "nobody@example.com";
        var identity = new Mock<IIdentityService>();
        identity
            .Setup(service => service.CreatePasswordResetTokenAsync(unknownEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);
        var useCase = new CreatePasswordResetTokenUseCase(identity.Object);

        var response = await useCase.ExecuteAsync(new CreatePasswordResetTokenCommand($" {unknownEmail} "), CancellationToken.None);

        response.Error!.Code.ShouldBe(AuthenticationErrorCodes.AccountNotFound);
    }
}
