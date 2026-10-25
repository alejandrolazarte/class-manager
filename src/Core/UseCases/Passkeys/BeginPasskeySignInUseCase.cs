using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Passkeys;

public sealed record BeginPasskeySignInCommand : ICommand;

public sealed class BeginPasskeySignInUseCase(IPasskeyIdentityService passkeyIdentityService)
    : IUseCase<BeginPasskeySignInCommand, PasskeyCeremonyResponse>
{
    public async Task<Result<PasskeyCeremonyResponse>> ExecuteAsync(BeginPasskeySignInCommand command, CancellationToken cancellationToken) =>
        PasskeyCeremonyResponse.From(await passkeyIdentityService.BeginSignInAsync(cancellationToken));
}
