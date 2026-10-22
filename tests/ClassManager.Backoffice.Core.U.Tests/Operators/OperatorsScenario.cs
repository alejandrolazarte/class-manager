using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Users;
using ClassManager.Tenancy;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Backoffice.Core.U.Tests.Operators;

internal sealed class OperatorsScenario
{
    public const string BranchName = "Laztty";

    public static readonly DateTimeOffset Now = new(2026, 10, 20, 12, 0, 0, TimeSpan.Zero);

    public static readonly ExternalIdentity Identity = new("google", "google-subject-2", "person@example.com");

    private readonly FakeTimeProvider _timeProvider = new(Now);

    public OperatorsScenario()
    {
        Operators.BranchNames[BranchId] = BranchName;
        CurrentUser = new FixedCurrentUser(CurrentOperatorAccountId, CurrentOperatorUserId);
    }

    public Guid BranchId { get; } = Guid.CreateVersion7();

    public Guid CurrentOperatorAccountId { get; } = Guid.CreateVersion7();

    public Guid CurrentOperatorUserId { get; } = Guid.CreateVersion7();

    public InMemoryBackofficeLoginRepository Logins { get; } = new();

    public InMemoryPlatformOperatorDirectory Operators { get; } = new();

    public RecordingTenantScope TenantScope { get; } = new();

    public SavingUnitOfWork UnitOfWork { get; } = new();

    public RecordingSessionService Sessions { get; } = new();

    public FixedCurrentUser CurrentUser { get; }

    public BackofficeAccessRequest WithAccessRequest()
    {
        var accessRequest = BackofficeAccessRequest.Create(Identity, "K7-3QF", Now.AddMinutes(-5));
        Logins.AccessRequests.Add(accessRequest);
        return accessRequest;
    }

    public BusinessMember WithTeamMember(string fullName, BusinessRole role, Guid? accountId)
    {
        var user = accountId is { } account
            ? User.CreateForAccount(account, fullName, null, Now)
            : User.Create(fullName, null, Now).Value!;
        var member = BusinessMember.Create(BranchId, user, MemberRole.System(role), instructorId: null).Value!;
        Operators.TeamMembers.Add(member);
        return member;
    }

    public BackofficeLogin WithLogin(Guid accountId, string providerKey)
    {
        var login = BackofficeLogin.Enable(Identity with { ProviderKey = providerKey }, accountId, Now.AddDays(-3), enabledByUserId: null);
        Logins.Logins.Add(login);
        return login;
    }

    public Task<Result<OperatorListResponse>> ListAsync() =>
        new ListOperatorsUseCase(Logins, Operators, CurrentUser)
            .ExecuteAsync(new ListOperatorsQuery(), CancellationToken.None);

    public Task<Result<OperatorLoginResponse>> EnableAsync(Guid accessRequestId, Guid memberId, BusinessRole role) =>
        new EnableOperatorUseCase(Logins, Operators, TenantScope, UnitOfWork, CurrentUser, _timeProvider)
            .ExecuteAsync(new EnableOperatorCommand(accessRequestId, memberId, role), CancellationToken.None);

    public Task<Result<OperatorLoginResponse>> DisableAsync(Guid loginId) =>
        new DisableOperatorUseCase(Logins, Sessions, CurrentUser)
            .ExecuteAsync(new DisableOperatorCommand(loginId), CancellationToken.None);
}

internal sealed class RecordingTenantScope : ITenantScope
{
    public Guid? EstablishedTenantId { get; private set; }

    public void Establish(Guid tenantId) => EstablishedTenantId = tenantId;
}

internal sealed class SavingUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveCount++;
        return Task.CompletedTask;
    }

    public Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken) =>
        throw new NotSupportedException();
}

internal sealed class RecordingSessionService : IBackofficeSessionService
{
    public List<Guid> RevokedAccountIds { get; } = [];

    public Task RevokeAllAsync(Guid accountId, CancellationToken cancellationToken)
    {
        RevokedAccountIds.Add(accountId);
        return Task.CompletedTask;
    }
}

internal sealed class FixedCurrentUser(Guid accountId, Guid userId) : ICurrentUser
{
    public Guid? AccountId { get; } = accountId;

    public Guid? UserId { get; } = userId;
}
