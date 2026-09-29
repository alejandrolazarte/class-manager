using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record SignInCommand(string? Email, string? Password);

public sealed class SignInUseCase(
    IIdentityService identityService,
    ITokenService tokenService,
    IBranchDirectory branchDirectory,
    IFamilyDirectory familyDirectory)
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

        var branch = await branchDirectory.FindDefaultAsync(verification.UserId, cancellationToken);
        if (branch is not null)
        {
            return TokenResponse.From(await tokenService.IssueAsync(
                new SessionUser(verification.UserId, verification.Email, branch.BusinessId, branch.RoleName),
                cancellationToken));
        }

        var family = await familyDirectory.FindDefaultAsync(verification.UserId, cancellationToken);
        if (family is null)
        {
            return InvalidCredentials();
        }

        return TokenResponse.From(await tokenService.IssueAsync(
            new SessionUser(verification.UserId, verification.Email, family.BusinessId, AccountKinds.FamilyRoleName, AccountKinds.Family),
            cancellationToken));
    }

    private static Result<TokenResponse> InvalidCredentials() =>
        Result.Unauthorized<TokenResponse>(InvalidCredentialsMessage, AuthenticationErrorCodes.InvalidCredentials);
}
