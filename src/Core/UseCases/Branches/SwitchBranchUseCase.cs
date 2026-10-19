using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.UseCases.Branches;

public sealed record SwitchBranchCommand([property: AuditSecret] string? RefreshToken, Guid? BusinessId, [property: AuditPlain] string? Kind = null) : ICommand;

public sealed class SwitchBranchUseCase(ITokenService tokenService) : IUseCase<SwitchBranchCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> ExecuteAsync(SwitchBranchCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken)
            || command.BusinessId is not { } businessId
            || (command.Kind is not null && !AccountKinds.IsTeam(command.Kind) && !AccountKinds.IsStudent(command.Kind)))
        {
            return Unavailable();
        }

        var tokens = await tokenService.SwitchBusinessAsync(command.RefreshToken, businessId, command.Kind, cancellationToken);

        return tokens.IsFailure ? Unavailable() : TokenResponse.From(tokens.Value!);
    }

    private static Result<TokenResponse> Unavailable() =>
        Result.Forbidden<TokenResponse>(BranchErrorCodes.UnavailableMessage, BranchErrorCodes.Unavailable);
}
