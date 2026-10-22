using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClassPackRepository
{
    void Add(ClassPack classPack);

    Task<ClassPack?> GetByIdAsync(long classPackId, CancellationToken cancellationToken);

    Task<ClassPack?> GetForUpdateAsync(long classPackId, CancellationToken cancellationToken);

    Task<ClassPack?> FindByNameAsync(string name, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassPack>> ListAsync(bool includeInactive, CancellationToken cancellationToken);
}
