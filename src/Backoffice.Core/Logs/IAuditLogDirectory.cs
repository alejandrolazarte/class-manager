namespace ClassManager.Backoffice.Core.Logs;

public interface IAuditLogDirectory
{
    Task<IReadOnlyList<Guid>> ListBranchIdsAsync(Guid brandId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, LoggedBranch>> FindBranchesAsync(IReadOnlyCollection<Guid> branchIds, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, string>> FindUserNamesAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken);
}
