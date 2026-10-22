using ClassManager.Core.Domain.Roles;

namespace ClassManager.Core.Abstractions.Persistence;

public interface ICustomRoleRepository
{
    void Add(CustomRole role);

    void Remove(CustomRole role);

    Task<CustomRole?> GetByIdAsync(long roleId, CancellationToken cancellationToken);

    Task<CustomRole?> GetForUpdateAsync(long roleId, CancellationToken cancellationToken);

    Task<IReadOnlyList<CustomRole>> ListAsync(CancellationToken cancellationToken);
}
