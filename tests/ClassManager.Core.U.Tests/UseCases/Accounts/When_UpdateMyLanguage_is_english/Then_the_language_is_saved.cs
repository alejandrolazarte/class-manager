using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_UpdateMyLanguage_is_english;

public sealed class Then_the_language_is_saved
{
    [Fact]
    public async Task Then_the_language_is_saved_Run()
    {
        var userId = Guid.CreateVersion7();
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.UserId).Returns(userId);
        var identity = new Mock<IIdentityService>();
        identity
            .Setup(service => service.UpdatePreferredLanguageAsync(userId, AccountLanguage.English, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        identity
            .Setup(service => service.ListAccountsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserAccount(userId, TestData.OwnerEmail, TestData.OwnerFullName, TestData.OwnerBirthDate, AccountLanguage.English)]);

        var response = await new UpdateMyLanguageUseCase(currentUser.Object, identity.Object, Mock.Of<ILegalTermsAcceptanceRepository>())
            .ExecuteAsync(new UpdateMyLanguageCommand(AccountLanguage.English), CancellationToken.None);

        response.Value!.PreferredLanguage.ShouldBe(AccountLanguage.English);
        identity.Verify(service => service.UpdatePreferredLanguageAsync(userId, AccountLanguage.English, It.IsAny<CancellationToken>()));
    }
}
