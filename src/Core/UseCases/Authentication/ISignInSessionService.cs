using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Authentication;

public interface ISignInSessionService
{
    Task<Result<TokenResponse>> StartAsync(CredentialVerification verification, CancellationToken cancellationToken);
}
