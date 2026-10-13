using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_GetMyAccount_for_an_account_that_accepted_an_older_version;

public sealed class Then_the_current_legal_terms_are_not_accepted
{
    private const string OlderVersion = "2020-01-01";

    [Fact]
    public async Task Then_the_current_legal_terms_are_not_accepted_Run()
    {
        var userId = Guid.CreateVersion7();
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.UserId).Returns(userId);
        var identity = new Mock<IIdentityService>();
        identity
            .Setup(service => service.ListAccountsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserAccount(userId, TestData.OwnerEmail, TestData.OwnerFullName, TestData.OwnerBirthDate)]);
        var acceptances = new Mock<ILegalTermsAcceptanceRepository>();
        acceptances
            .Setup(repository => repository.HasAcceptedAsync(userId, OlderVersion, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var response = await new GetMyAccountUseCase(currentUser.Object, identity.Object, acceptances.Object)
            .ExecuteAsync(new GetMyAccountQuery(), CancellationToken.None);

        response.Value!.HasAcceptedCurrentLegalTerms.ShouldBeFalse();
    }
}
