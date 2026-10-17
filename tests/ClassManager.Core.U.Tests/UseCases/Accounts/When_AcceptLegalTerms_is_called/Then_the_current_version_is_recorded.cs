using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.UseCases.Accounts;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_AcceptLegalTerms_is_called;

public sealed class Then_the_current_version_is_recorded
{
    [Fact]
    public async Task Then_the_current_version_is_recorded_Run()
    {
        var userId = Guid.CreateVersion7();
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.AccountId).Returns(userId);
        var acceptances = new Mock<ILegalTermsAcceptanceRepository>();
        LegalTermsAcceptance? added = null;
        acceptances
            .Setup(repository => repository.Add(It.IsAny<LegalTermsAcceptance>()))
            .Callback<LegalTermsAcceptance>(acceptance => added = acceptance);
        var unitOfWork = new Mock<IUnitOfWork>();
        var useCase = new AcceptLegalTermsUseCase(currentUser.Object, acceptances.Object, unitOfWork.Object, new FakeTimeProvider(TestData.Now));

        var response = await useCase.ExecuteAsync(new AcceptLegalTermsCommand(), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        added!.UserId.ShouldBe(userId);
        added.Version.ShouldBe(LegalTerms.CurrentVersion);
        added.AcceptedOn.ShouldBe(TestData.Now);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
