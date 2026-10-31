using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClassPackRepository
{
    void Add(ClassPack classPack);

    void Add(ClassPackPriceChange priceChange);

    void Remove(ClassPackPriceChange priceChange);

    Task<IReadOnlyList<ClassPackPriceChange>> ListPriceChangesAsync(CancellationToken cancellationToken);

    Task<ClassPackPriceChange?> FindPriceChangeForUpdateAsync(long classPackId, DateOnly effectiveFrom, CancellationToken cancellationToken);

    Task<ClassPack?> GetByIdAsync(long classPackId, CancellationToken cancellationToken);

    Task<ClassPack?> GetForUpdateAsync(long classPackId, CancellationToken cancellationToken);

    Task<ClassPack?> FindByNameAsync(string name, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassPack>> ListAsync(bool includeInactive, CancellationToken cancellationToken);
}
