using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.UseCases.Branches;

public sealed record SwitchBranchCommand(string? RefreshToken, Guid? BusinessId);

public sealed class SwitchBranchUseCase(ITokenService tokenService) : IUseCase<SwitchBranchCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> ExecuteAsync(SwitchBranchCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken) || command.BusinessId is not { } businessId)
        {
            return Unavailable();
        }

        var tokens = await tokenService.SwitchBusinessAsync(command.RefreshToken, businessId, cancellationToken);

        return tokens.IsFailure ? Unavailable() : TokenResponse.From(tokens.Value!);
    }

    private static Result<TokenResponse> Unavailable() =>
        Result.Forbidden<TokenResponse>(BranchErrorCodes.UnavailableMessage, BranchErrorCodes.Unavailable);
}
