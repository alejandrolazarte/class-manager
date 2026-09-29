using ClassManager.Core.Domain.Roles;

namespace ClassManager.Core.Abstractions.Persistence;

public interface ICustomRoleRepository
{
    void Add(CustomRole role);

    void Remove(CustomRole role);

    Task<CustomRole?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken);

    Task<CustomRole?> GetForUpdateAsync(Guid roleId, CancellationToken cancellationToken);

    Task<IReadOnlyList<CustomRole>> ListAsync(CancellationToken cancellationToken);
}
