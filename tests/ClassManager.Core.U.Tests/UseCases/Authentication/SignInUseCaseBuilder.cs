using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.U.Tests.UseCases.Authentication;

internal sealed class SignInUseCaseBuilder
{
    public Guid UserId { get; } = Guid.CreateVersion7();
    public BranchAccess Branch { get; } =
        new(Guid.CreateVersion7(), TestData.BusinessName, Guid.CreateVersion7(), BusinessRole.BranchOwner, IsBrandOwner: true);

    public Mock<IIdentityService> Identity { get; } = new();
    public Mock<ITokenService> Tokens { get; } = new();
    public Mock<IBranchDirectory> Branches { get; } = new();
    public Mock<IStudentAppDirectory> StudentApp { get; } = new();
    public StudentAppLink Student { get; } = new(Guid.CreateVersion7(), TestData.BusinessName, Guid.CreateVersion7());

    public SignInUseCaseBuilder()
    {
        Identity
            .Setup(service => service.VerifyCredentialsAsync(TestData.OwnerEmail, TestData.OwnerPassword, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CredentialVerification.Verified(UserId, TestData.OwnerEmail));
        Branches
            .Setup(directory => directory.FindDefaultAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Branch);
        Tokens
            .Setup(service => service.IssueAsync(It.IsAny<SessionUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.IssuedTokens());
    }

    public static SignInCommand ValidCommand() => new(TestData.OwnerEmail, TestData.OwnerPassword);

    public void WithoutBranch() =>
        Branches.Setup(directory => directory.FindDefaultAsync(UserId, It.IsAny<CancellationToken>())).ReturnsAsync((BranchAccess?)null);

    public void WithStudentAccount() =>
        StudentApp.Setup(directory => directory.FindDefaultAsync(UserId, It.IsAny<CancellationToken>())).ReturnsAsync(Student);

    public SignInUseCase Build() => new(Identity.Object, Tokens.Object, Branches.Object, StudentApp.Object);
}
