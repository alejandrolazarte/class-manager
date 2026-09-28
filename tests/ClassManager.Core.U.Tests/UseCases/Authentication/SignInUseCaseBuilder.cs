using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.U.Tests.UseCases.Authentication;

internal sealed class SignInUseCaseBuilder
{
    public Guid UserId { get; } = Guid.CreateVersion7();
    public BusinessMember Member { get; }

    public Mock<IIdentityService> Identity { get; } = new();
    public Mock<ITokenService> Tokens { get; } = new();
    public Mock<IBusinessMemberRepository> BusinessMembers { get; } = new();

    public SignInUseCaseBuilder()
    {
        Member = BusinessMember.CreateBranchOwner(Guid.CreateVersion7(), UserId);
        Identity
            .Setup(service => service.VerifyCredentialsAsync(TestData.OwnerEmail, TestData.OwnerPassword, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CredentialVerification.Verified(UserId, TestData.OwnerEmail));
        BusinessMembers
            .Setup(repository => repository.FindByUserIdAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Member);
        Tokens
            .Setup(service => service.IssueAsync(It.IsAny<SessionUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.IssuedTokens());
    }

    public static SignInCommand ValidCommand() => new(TestData.OwnerEmail, TestData.OwnerPassword);

    public SignInUseCase Build() => new(Identity.Object, Tokens.Object, BusinessMembers.Object);
}
