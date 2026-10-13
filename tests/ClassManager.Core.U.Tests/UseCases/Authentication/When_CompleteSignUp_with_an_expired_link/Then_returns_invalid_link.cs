using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_CompleteSignUp_with_an_expired_link;

public sealed class Then_returns_invalid_link
{
    [Fact]
    public async Task Then_returns_invalid_link_Run()
    {
        var builder = new CompleteSignUpUseCaseBuilder();
        var expiredSignUp = TestData.PendingSignUp(CompleteSignUpUseCaseBuilder.TokenHash, TestData.Now - PendingSignUp.Lifetime);
        builder.PendingSignUps
            .Setup(repository => repository.FindForUpdateByTokenHashAsync(CompleteSignUpUseCaseBuilder.TokenHash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiredSignUp);

        var response = await builder.Build().ExecuteAsync(CompleteSignUpUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(AuthenticationErrorCodes.InvalidSignUpToken);
        builder.Identity.Verify(
            service => service.CreateOwnerAsync(It.IsAny<OwnerAccount>(), It.IsAny<LegalTermsAcceptance?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
