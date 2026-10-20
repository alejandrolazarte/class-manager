namespace ClassManager.Backoffice.Core.Operators;

public interface IBackofficeLoginRepository
{
    Task<BackofficeLogin?> FindAsync(string provider, string providerKey, CancellationToken cancellationToken);

    Task<bool> HasEnabledLoginAsync(Guid accountId, CancellationToken cancellationToken);

    Task<BackofficeAccessRequest?> FindAccessRequestAsync(string provider, string providerKey, CancellationToken cancellationToken);

    void AddAccessRequest(BackofficeAccessRequest accessRequest);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
