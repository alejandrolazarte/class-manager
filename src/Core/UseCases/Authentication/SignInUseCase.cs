using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record SignInCommand(string? Email, string? Password);

public sealed class SignInUseCase(
    IIdentityService identityService,
    ITokenService tokenService,
    IBusinessMemberRepository businessMemberRepository)
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

        var member = await businessMemberRepository.FindByUserIdAsync(verification.UserId, cancellationToken);
        if (member is null)
        {
            return InvalidCredentials();
        }

        var tokens = await tokenService.IssueAsync(
            new SessionUser(verification.UserId, verification.Email, member.TenantId, member.Role),
            cancellationToken);

        return TokenResponse.From(tokens);
    }

    private static Result<TokenResponse> InvalidCredentials() =>
        Result.Unauthorized<TokenResponse>(InvalidCredentialsMessage, AuthenticationErrorCodes.InvalidCredentials);
}
