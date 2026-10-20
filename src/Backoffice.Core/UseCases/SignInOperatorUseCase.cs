using ClassManager.Backoffice.Core.Operators;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Backoffice.Core.UseCases;

public sealed record SignInOperatorCommand(
    [property: AuditPlain] string Provider,
    [property: AuditPersonal] string ProviderKey,
    [property: AuditPersonal] string Email) : ICommand;

public sealed record OperatorSignInResponse(TokenResponse? Tokens, string? AccessRequestCode);

public sealed class SignInOperatorUseCase(
    IBackofficeLoginRepository loginRepository,
    IPlatformOperatorDirectory operatorDirectory,
    ITokenService tokenService,
    TimeProvider timeProvider)
    : IUseCase<SignInOperatorCommand, OperatorSignInResponse>
{
    public async Task<Result<OperatorSignInResponse>> ExecuteAsync(SignInOperatorCommand command, CancellationToken cancellationToken)
    {
        var identity = new ExternalIdentity(command.Provider, command.ProviderKey, command.Email);
        var login = await loginRepository.FindAsync(identity.Provider, identity.ProviderKey, cancellationToken);
        if (login is not null && await operatorDirectory.FindAsync(login.AccountId, cancellationToken) is { } platformOperator)
        {
            var tokens = await tokenService.IssueAsync(
                new SessionUser(login.AccountId, identity.Email, platformOperator.BusinessId, platformOperator.Role.ToString(), BackofficeSession.Kind),
                cancellationToken);
            return new OperatorSignInResponse(TokenResponse.From(tokens), AccessRequestCode: null);
        }

        return new OperatorSignInResponse(Tokens: null, await RequestAccessAsync(identity, cancellationToken));
    }

    private async Task<string> RequestAccessAsync(ExternalIdentity identity, CancellationToken cancellationToken)
    {
        var code = AccessRequestCodes.Create();
        var now = timeProvider.GetUtcNow();
        if (await loginRepository.FindAccessRequestAsync(identity.Provider, identity.ProviderKey, cancellationToken) is { } accessRequest)
        {
            accessRequest.Renew(identity, code, now);
        }
        else
        {
            loginRepository.AddAccessRequest(BackofficeAccessRequest.Create(identity, code, now));
        }

        await loginRepository.SaveChangesAsync(cancellationToken);
        return code;
    }
}
