namespace ClassManager.Backoffice.Core.Logs;

public interface IAuditLogDirectory
{
    Task<IReadOnlyList<long>> ListBranchIdsAsync(long brandId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, LoggedBranch>> FindBranchesAsync(IReadOnlyCollection<long> branchIds, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, string>> FindUserNamesAsync(IReadOnlyCollection<long> userIds, CancellationToken cancellationToken);
}
