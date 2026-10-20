using ClassManager.Backoffice.Core.Operators;
using ClassManager.Backoffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Backoffice.Infrastructure.Operators;

internal sealed class BackofficeLoginRepository(BackofficeDbContext context) : IBackofficeLoginRepository
{
    public Task<BackofficeLogin?> FindAsync(string provider, string providerKey, CancellationToken cancellationToken) =>
        context.BackofficeLogins
            .AsNoTracking()
            .SingleOrDefaultAsync(login => login.Provider == provider && login.ProviderKey == providerKey, cancellationToken);

    public Task<bool> HasEnabledLoginAsync(Guid accountId, CancellationToken cancellationToken) =>
        context.BackofficeLogins.AnyAsync(login => login.AccountId == accountId, cancellationToken);

    public Task<BackofficeAccessRequest?> FindAccessRequestAsync(string provider, string providerKey, CancellationToken cancellationToken) =>
        context.BackofficeAccessRequests
            .SingleOrDefaultAsync(request => request.Provider == provider && request.ProviderKey == providerKey, cancellationToken);

    public void AddAccessRequest(BackofficeAccessRequest accessRequest) => context.BackofficeAccessRequests.Add(accessRequest);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
