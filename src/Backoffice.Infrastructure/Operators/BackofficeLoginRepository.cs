using ClassManager.Backoffice.Core.Operators;
using ClassManager.Backoffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Backoffice.Infrastructure.Operators;

internal sealed class BackofficeLoginRepository(BackofficeDbContext context) : IBackofficeLoginRepository
{
    public Task<BackofficeLogin?> FindAsync(string provider, string providerKey, CancellationToken cancellationToken) =>
        context.BackofficeLogins
            .SingleOrDefaultAsync(login => login.Provider == provider && login.ProviderKey == providerKey, cancellationToken);

    public Task<BackofficeLogin?> FindByIdAsync(Guid loginId, CancellationToken cancellationToken) =>
        context.BackofficeLogins.SingleOrDefaultAsync(login => login.Id == loginId, cancellationToken);

    public Task<bool> HasEnabledLoginAsync(Guid accountId, CancellationToken cancellationToken) =>
        context.BackofficeLogins.AnyAsync(login => login.AccountId == accountId, cancellationToken);

    public async Task<IReadOnlyList<BackofficeLogin>> ListAsync(CancellationToken cancellationToken) =>
        await context.BackofficeLogins.AsNoTracking().ToListAsync(cancellationToken);

    public Task<BackofficeAccessRequest?> FindAccessRequestAsync(string provider, string providerKey, CancellationToken cancellationToken) =>
        context.BackofficeAccessRequests
            .SingleOrDefaultAsync(request => request.Provider == provider && request.ProviderKey == providerKey, cancellationToken);

    public Task<BackofficeAccessRequest?> FindAccessRequestByIdAsync(Guid accessRequestId, CancellationToken cancellationToken) =>
        context.BackofficeAccessRequests.SingleOrDefaultAsync(request => request.Id == accessRequestId, cancellationToken);

    public async Task<IReadOnlyList<BackofficeAccessRequest>> ListAccessRequestsAsync(CancellationToken cancellationToken) =>
        await context.BackofficeAccessRequests.AsNoTracking().ToListAsync(cancellationToken);

    public void AddAccessRequest(BackofficeAccessRequest accessRequest) => context.BackofficeAccessRequests.Add(accessRequest);

    public void Add(BackofficeLogin login) => context.BackofficeLogins.Add(login);

    public void Remove(BackofficeLogin login) => context.BackofficeLogins.Remove(login);

    public void Remove(BackofficeAccessRequest accessRequest) => context.BackofficeAccessRequests.Remove(accessRequest);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
