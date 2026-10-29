using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.UseCases.Accounts;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Accounts;

internal sealed class DeleteMyAccountUseCaseBuilder
{
    public const string CurrentPassword = "a long passphrase";

    public static readonly DateTimeOffset Now = new(2026, 10, 17, 12, 0, 0, TimeSpan.Zero);

    public Guid AccountId { get; } = Guid.CreateVersion7();

    public Mock<IIdentityService> Identity { get; } = new();

    public Mock<IOrganizationMemberRepository> OrganizationMembers { get; } = new();

    public Mock<IAccountLinkRepository> AccountLinks { get; } = new();

    public Mock<IUserPhotoRepository> Photos { get; } = new();

    public DeleteMyAccountUseCaseBuilder WithCorrectPassword()
    {
        Identity
            .Setup(service => service.IsPasswordCorrectAsync(AccountId, CurrentPassword, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        Identity
            .Setup(service => service.DeleteAccountAsync(AccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        Photos
            .Setup(repository => repository.DeleteEveryPhotoOfAccountAsync(AccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        return this;
    }

    public DeleteMyAccountUseCaseBuilder AsTheOnlyBrandOwner()
    {
        OrganizationMembers
            .Setup(repository => repository.IsTheOnlyBrandOwnerOfABrandAsync(AccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        return this;
    }

    public DeleteMyAccountUseCase Build()
    {
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.AccountId).Returns(AccountId);
        return new DeleteMyAccountUseCase(
            currentUser.Object,
            Identity.Object,
            OrganizationMembers.Object,
            AccountLinks.Object,
            Photos.Object,
            new Mock<IDocumentStorageService>().Object,
            new FakeTimeProvider(Now));
    }

    public void VerifyNothingWasDeleted()
    {
        Identity.Verify(service => service.DeleteAccountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        AccountLinks.Verify(
            repository => repository.UnlinkFromEveryBusinessAsync(It.IsAny<Guid>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
            Times.Never);
        Photos.Verify(repository => repository.DeleteEveryPhotoOfAccountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
