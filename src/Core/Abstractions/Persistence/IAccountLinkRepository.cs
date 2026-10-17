namespace ClassManager.Core.Abstractions.Persistence;

public interface IAccountLinkRepository
{
    Task UnlinkFromEveryBusinessAsync(Guid accountId, DateTimeOffset unlinkedAt, CancellationToken cancellationToken);
}
