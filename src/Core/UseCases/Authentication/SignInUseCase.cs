using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record SignInCommand(string? Email, string? Password) : ICommand;

public sealed class SignInUseCase(
    IIdentityService identityService,
    ITokenService tokenService,
    IBranchDirectory branchDirectory,
    IStudentAppDirectory studentAppDirectory)
    : IUseCase<SignInCommand, TokenResponse>
{
    public const string InvalidCredentialsMessage = "Wrong email or password.";
    public const string LockedOutMessage = "Too many failed attempts. Try again later.";

    public async Task<Result<TokenResponse>> ExecuteAsync(SignInCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrEmpty(command.Password))
        {
            return InvalidCredentials();
        }

        var verification = await identityService.VerifyCredentialsAsync(command.Email.Trim(), command.Password, cancellationToken);
        if (verification.Status == CredentialVerificationStatus.LockedOut)
        {
            return Result.Locked<TokenResponse>(LockedOutMessage, AuthenticationErrorCodes.LockedOut);
        }

        if (verification.Status != CredentialVerificationStatus.Verified)
        {
            return InvalidCredentials();
        }

        var lastSessionUser = await FindLastSessionUserAsync(verification.UserId, verification.Email, cancellationToken);
        if (lastSessionUser is not null)
        {
            return TokenResponse.From(await tokenService.IssueAsync(lastSessionUser, cancellationToken));
        }

        var branch = await branchDirectory.FindDefaultAsync(verification.UserId, cancellationToken);
        if (branch is not null)
        {
            return TokenResponse.From(await tokenService.IssueAsync(TeamUser(verification.UserId, verification.Email, branch), cancellationToken));
        }

        var student = await studentAppDirectory.FindDefaultAsync(verification.UserId, cancellationToken);
        if (student is null)
        {
            return InvalidCredentials();
        }

        return TokenResponse.From(await tokenService.IssueAsync(StudentUser(verification.UserId, verification.Email, student), cancellationToken));
    }

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

    private static SessionUser TeamUser(Guid userId, string email, BranchAccess branch) =>
        new(userId, email, branch.BusinessId, branch.RoleName);

    private static SessionUser StudentUser(Guid userId, string email, StudentAppLink student) =>
        new(userId, email, student.BusinessId, AccountKinds.StudentRoleName, AccountKinds.Student);

    private static Result<TokenResponse> InvalidCredentials() =>
        Result.Unauthorized<TokenResponse>(InvalidCredentialsMessage, AuthenticationErrorCodes.InvalidCredentials);
}
