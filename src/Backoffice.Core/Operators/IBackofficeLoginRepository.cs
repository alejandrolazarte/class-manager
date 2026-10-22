namespace ClassManager.Backoffice.Core.Operators;

public interface IBackofficeLoginRepository
{
    Task<BackofficeLogin?> FindAsync(string provider, string providerKey, CancellationToken cancellationToken);

    Task<BackofficeLogin?> FindByIdAsync(long loginId, CancellationToken cancellationToken);

    Task<bool> HasEnabledLoginAsync(Guid accountId, CancellationToken cancellationToken);

    Task<IReadOnlyList<BackofficeLogin>> ListAsync(CancellationToken cancellationToken);

    Task<BackofficeAccessRequest?> FindAccessRequestAsync(string provider, string providerKey, CancellationToken cancellationToken);

    Task<BackofficeAccessRequest?> FindAccessRequestByIdAsync(long accessRequestId, CancellationToken cancellationToken);

    Task<IReadOnlyList<BackofficeAccessRequest>> ListAccessRequestsAsync(CancellationToken cancellationToken);

    void AddAccessRequest(BackofficeAccessRequest accessRequest);

    void Add(BackofficeLogin login);

    void Remove(BackofficeLogin login);

    void Remove(BackofficeAccessRequest accessRequest);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
