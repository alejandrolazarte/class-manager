namespace ClassManager.Core.Abstractions.Security;

public interface IBranchDirectory
{
    Task<BranchAccess?> FindDefaultAsync(Guid userId, CancellationToken cancellationToken);

    Task<BranchAccess?> FindAsync(Guid userId, Guid businessId, CancellationToken cancellationToken);

    Task<IReadOnlyList<BranchAccess>> ListAsync(Guid userId, CancellationToken cancellationToken);
}
