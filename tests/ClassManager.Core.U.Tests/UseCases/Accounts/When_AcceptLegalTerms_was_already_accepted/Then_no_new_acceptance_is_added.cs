using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.UseCases.Accounts;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_AcceptLegalTerms_was_already_accepted;

public sealed class Then_no_new_acceptance_is_added
{
    [Fact]
    public async Task Then_no_new_acceptance_is_added_Run()
    {
        var userId = Guid.CreateVersion7();
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.AccountId).Returns(userId);
        var acceptances = new Mock<ILegalTermsAcceptanceRepository>();
        acceptances
            .Setup(repository => repository.HasAcceptedAsync(userId, LegalTerms.CurrentVersion, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var useCase = new AcceptLegalTermsUseCase(currentUser.Object, acceptances.Object, Mock.Of<IUnitOfWork>(), new FakeTimeProvider(TestData.Now));

        var response = await useCase.ExecuteAsync(new AcceptLegalTermsCommand(), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        acceptances.Verify(repository => repository.Add(It.IsAny<LegalTermsAcceptance>()), Times.Never);
    }
}
