using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_ResetPassword_without_token;

public sealed class Then_returns_invalid_link
{
    [Fact]
    public async Task Then_returns_invalid_link_Run()
    {
        var identity = new Mock<IIdentityService>();
        var useCase = new ResetPasswordUseCase(identity.Object);

        var response = await useCase.ExecuteAsync(new ResetPasswordCommand(" ", TestData.OwnerPassword), CancellationToken.None);

        response.Error!.Code.ShouldBe(AuthenticationErrorCodes.InvalidPasswordResetToken);
        identity.VerifyNoOtherCalls();
    }
}
