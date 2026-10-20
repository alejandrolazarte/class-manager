using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Backoffice.Core.U.Tests.Operators;

internal sealed class SignInScenario
{
    public static readonly DateTimeOffset Now = new(2026, 10, 20, 12, 0, 0, TimeSpan.Zero);

    public static readonly ExternalIdentity Identity = new("google", "google-subject-1", "operator@example.com");

    public Guid AccountId { get; } = Guid.CreateVersion7();

    public Guid BusinessId { get; } = Guid.CreateVersion7();

    public InMemoryBackofficeLoginRepository Logins { get; } = new();

    public InMemoryPlatformOperatorDirectory Operators { get; } = new();

    public RecordingTokenService Tokens { get; } = new();

    public SignInScenario WithEnabledLogin()
    {
        Logins.Logins.Add(BackofficeLogin.Enable(Identity, AccountId, Now, enabledByUserId: null));
        return this;
    }

    public SignInScenario WithPlatformRole(BusinessRole role)
    {
        Operators.Operators.Add(new PlatformOperator(AccountId, Guid.CreateVersion7(), BusinessId, role));
        return this;
    }

    public Task<Result<OperatorSignInResponse>> SignInAsync() =>
        new SignInOperatorUseCase(Logins, Operators, Tokens, new FakeTimeProvider(Now))
            .ExecuteAsync(new SignInOperatorCommand(Identity.Provider, Identity.ProviderKey, Identity.Email), CancellationToken.None);
}

internal sealed class InMemoryBackofficeLoginRepository : IBackofficeLoginRepository
{
    public List<BackofficeLogin> Logins { get; } = [];

    public List<BackofficeAccessRequest> AccessRequests { get; } = [];

    public Task<BackofficeLogin?> FindAsync(string provider, string providerKey, CancellationToken cancellationToken) =>
        Task.FromResult(Logins.SingleOrDefault(login => login.Provider == provider && login.ProviderKey == providerKey));

    public Task<bool> HasEnabledLoginAsync(Guid accountId, CancellationToken cancellationToken) =>
        Task.FromResult(Logins.Any(login => login.AccountId == accountId));

    public Task<BackofficeAccessRequest?> FindAccessRequestAsync(string provider, string providerKey, CancellationToken cancellationToken) =>
        Task.FromResult(AccessRequests.SingleOrDefault(request => request.Provider == provider && request.ProviderKey == providerKey));

    public void AddAccessRequest(BackofficeAccessRequest accessRequest) => AccessRequests.Add(accessRequest);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

internal sealed class InMemoryPlatformOperatorDirectory : IPlatformOperatorDirectory
{
    public List<PlatformOperator> Operators { get; } = [];

    public Task<PlatformOperator?> FindAsync(Guid accountId, CancellationToken cancellationToken) =>
        Task.FromResult(Operators.FirstOrDefault(platformOperator => platformOperator.AccountId == accountId));
}

internal sealed class RecordingTokenService : ITokenService
{
    public List<SessionUser> IssuedTo { get; } = [];

    public Task<IssuedTokens> IssueAsync(SessionUser user, CancellationToken cancellationToken)
    {
        IssuedTo.Add(user);
        return Task.FromResult(new IssuedTokens("access", SignInScenario.Now.AddMinutes(10), "refresh", SignInScenario.Now.AddHours(8), user.Kind));
    }

    public Task<Result<IssuedTokens>> RefreshAsync(string refreshToken, CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<Result<IssuedTokens>> SwitchBusinessAsync(string refreshToken, Guid businessId, string? kind, CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<LastSession?> FindLastSessionAsync(Guid userId, CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task RevokeAsync(string refreshToken, CancellationToken cancellationToken) =>
        throw new NotSupportedException();
}
