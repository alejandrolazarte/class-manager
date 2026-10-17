using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_UpdateMyLanguage_has_an_unsupported_language;

public sealed class Then_it_is_rejected
{
    private const string UnsupportedLanguage = "fr";

    [Fact]
    public async Task Then_it_is_rejected_Run()
    {
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.AccountId).Returns(Guid.CreateVersion7());
        var identity = new Mock<IIdentityService>();

        var response = await new UpdateMyLanguageUseCase(currentUser.Object, identity.Object, Mock.Of<ILegalTermsAcceptanceRepository>())
            .ExecuteAsync(new UpdateMyLanguageCommand(UnsupportedLanguage), CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.Validation);
        identity.Verify(
            service => service.UpdatePreferredLanguageAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
