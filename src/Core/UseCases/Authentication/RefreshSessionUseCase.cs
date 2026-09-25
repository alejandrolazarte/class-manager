using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record RefreshSessionCommand(string? RefreshToken);

public sealed class RefreshSessionUseCase(ITokenService tokenService) : IUseCase<RefreshSessionCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> ExecuteAsync(RefreshSessionCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return Result.Unauthorized<TokenResponse>(
                AuthenticationErrorCodes.InvalidRefreshTokenMessage, AuthenticationErrorCodes.InvalidRefreshToken);
        }

        var tokens = await tokenService.RefreshAsync(command.RefreshToken, cancellationToken);

        return tokens.IsFailure ? tokens.Error! : TokenResponse.From(tokens.Value!);
    }
}
