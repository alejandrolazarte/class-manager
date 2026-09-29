using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record SignInCommand(string? Email, string? Password);

public sealed class SignInUseCase(
    IIdentityService identityService,
    ITokenService tokenService,
    IBranchDirectory branchDirectory)
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
        if (branch is null)
        {
            return InvalidCredentials();
        }

        var tokens = await tokenService.IssueAsync(
            new SessionUser(verification.UserId, verification.Email, branch.BusinessId, branch.RoleName),
            cancellationToken);

        return TokenResponse.From(tokens);
    }

    private static Result<TokenResponse> InvalidCredentials() =>
        Result.Unauthorized<TokenResponse>(InvalidCredentialsMessage, AuthenticationErrorCodes.InvalidCredentials);
}
