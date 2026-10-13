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
        currentUser.Setup(user => user.UserId).Returns(userId);
        var identity = new Mock<IIdentityService>();
        identity
            .Setup(service => service.RecordLegalTermsAcceptanceAsync(userId, It.IsAny<LegalTermsAcceptance>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var useCase = new AcceptLegalTermsUseCase(currentUser.Object, identity.Object, new FakeTimeProvider(TestData.Now));

        var response = await useCase.ExecuteAsync(new AcceptLegalTermsCommand(), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        identity.Verify(
            service => service.RecordLegalTermsAcceptanceAsync(
                userId,
                new LegalTermsAcceptance(LegalTerms.CurrentVersion, TestData.Now),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
