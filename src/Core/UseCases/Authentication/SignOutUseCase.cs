using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record SignOutCommand(string? RefreshToken) : ICommand;

public sealed record SignOutResponse;

public sealed class SignOutUseCase(ITokenService tokenService) : IUseCase<SignOutCommand, SignOutResponse>
{
    public async Task<Result<SignOutResponse>> ExecuteAsync(SignOutCommand command, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            await tokenService.RevokeAsync(command.RefreshToken, cancellationToken);
        }

        return new SignOutResponse();
    }
}
