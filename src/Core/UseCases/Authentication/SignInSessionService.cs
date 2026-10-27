using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Authentication;

public sealed class SignInSessionService(
    ITokenService tokenService,
    IBranchDirectory branchDirectory,
    IStudentAppDirectory studentAppDirectory)
    : ISignInSessionService
{
    public const string InvalidCredentialsMessage = "Wrong email or password.";
    public const string LockedOutMessage = "Too many failed attempts. Try again later.";

    public async Task<Result<TokenResponse>> StartAsync(CredentialVerification verification, CancellationToken cancellationToken)
    {
        if (verification.Status == CredentialVerificationStatus.LockedOut)
        {
            return Result.Locked<TokenResponse>(LockedOutMessage, AuthenticationErrorCodes.LockedOut);
        }

        if (verification.Status != CredentialVerificationStatus.Verified)
        {
            return InvalidCredentials();
        }

        var sessionUser = await FindLastSessionUserAsync(verification.UserId, verification.Email, cancellationToken)
            ?? await FindDefaultSessionUserAsync(verification.UserId, verification.Email, cancellationToken);
        if (sessionUser is null)
        {
            return Result.Forbidden<TokenResponse>(AuthenticationErrorCodes.NoBusinessAccessMessage, AuthenticationErrorCodes.NoBusinessAccess);
        }

        return TokenResponse.From(await tokenService.IssueAsync(sessionUser, cancellationToken));
    }

    public static Result<TokenResponse> InvalidCredentials() =>
        Result.Unauthorized<TokenResponse>(InvalidCredentialsMessage, AuthenticationErrorCodes.InvalidCredentials);

    private async Task<SessionUser?> FindLastSessionUserAsync(Guid userId, string email, CancellationToken cancellationToken)
    {
        if (await tokenService.FindLastSessionAsync(userId, cancellationToken) is not { } lastSession)
        {
            return null;
        }

        if (AccountKinds.IsStudent(lastSession.Kind))
        {
            var student = await studentAppDirectory.FindAsync(userId, lastSession.BusinessId, cancellationToken);
            return student is null ? null : StudentUser(userId, email, student);
        }

        var branch = await branchDirectory.FindAsync(userId, lastSession.BusinessId, cancellationToken);
        return branch is null ? null : TeamUser(userId, email, branch);
    }

    private async Task<SessionUser?> FindDefaultSessionUserAsync(Guid userId, string email, CancellationToken cancellationToken)
    {
        var branch = await branchDirectory.FindDefaultAsync(userId, cancellationToken);
        if (branch is not null)
        {
            return TeamUser(userId, email, branch);
        }

        var student = await studentAppDirectory.FindDefaultAsync(userId, cancellationToken);
        return student is null ? null : StudentUser(userId, email, student);
    }

    private static SessionUser TeamUser(Guid userId, string email, BranchAccess branch) =>
        new(userId, email, branch.BusinessId, branch.RoleName);

    private static SessionUser StudentUser(Guid userId, string email, StudentAppLink student) =>
        new(userId, email, student.BusinessId, AccountKinds.StudentRoleName, AccountKinds.Student);
}
