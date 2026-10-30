using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_UpdateMyProfile_succeeds;

public sealed class Then_the_users_of_every_business_follow
{
    private const string NewFullName = "Ana María Pérez";

    [Fact]
    public async Task Then_the_users_of_every_business_follow_Run()
    {
        var accountId = Guid.CreateVersion7();
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.AccountId).Returns(accountId);
        var identity = new Mock<IIdentityService>();
        identity
            .Setup(service => service.UpdateProfileAsync(accountId, NewFullName, TestData.OwnerBirthDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        identity
            .Setup(service => service.ListAccountsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var users = new Mock<IUserRepository>();
        var useCase = new UpdateMyProfileUseCase(
            currentUser.Object, identity.Object, users.Object, Mock.Of<ILegalTermsAcceptanceRepository>(), TimeProvider.System);

        await useCase.ExecuteAsync(new UpdateMyProfileCommand($" {NewFullName} ", TestData.OwnerBirthDate), CancellationToken.None);

        users.Verify(repository => repository.ChangeProfileInEveryBusinessByAccountAsync(
            accountId, NewFullName, TestData.OwnerBirthDate, It.IsAny<CancellationToken>()));
    }
}
