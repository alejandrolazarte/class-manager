namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ClassPackRepository(AppDbContext context) : IClassPackRepository
{
    public void Add(ClassPack classPack) => context.ClassPacks.Add(classPack);

    public void Add(ClassPackPriceChange priceChange) => context.ClassPackPriceChanges.Add(priceChange);

    public void Remove(ClassPackPriceChange priceChange) => context.ClassPackPriceChanges.Remove(priceChange);

    public async Task<IReadOnlyList<ClassPackPriceChange>> ListPriceChangesAsync(CancellationToken cancellationToken) =>
        await context.ClassPackPriceChanges.AsNoTracking()
            .OrderBy(change => change.EffectiveFrom)
            .ToListAsync(cancellationToken);

    public Task<ClassPackPriceChange?> FindPriceChangeForUpdateAsync(long classPackId, DateOnly effectiveFrom, CancellationToken cancellationToken) =>
        context.ClassPackPriceChanges.FirstOrDefaultAsync(
            change => change.ClassPackId == classPackId && change.EffectiveFrom == effectiveFrom,
            cancellationToken);

    public Task<ClassPack?> GetByIdAsync(long classPackId, CancellationToken cancellationToken) =>
        context.ClassPacks.AsNoTracking().FirstOrDefaultAsync(classPack => classPack.Id == classPackId, cancellationToken);

    public Task<ClassPack?> GetForUpdateAsync(long classPackId, CancellationToken cancellationToken) =>
        context.ClassPacks.FirstOrDefaultAsync(classPack => classPack.Id == classPackId, cancellationToken);

    public Task<ClassPack?> FindByNameAsync(string name, CancellationToken cancellationToken) =>
        context.ClassPacks.AsNoTracking().FirstOrDefaultAsync(classPack => classPack.Name == name, cancellationToken);

    public async Task<IReadOnlyList<ClassPack>> ListAsync(bool includeInactive, CancellationToken cancellationToken) =>
        await context.ClassPacks.AsNoTracking()
            .Where(classPack => includeInactive || classPack.IsActive)
            .OrderByDescending(classPack => classPack.IsActive)
            .ThenBy(classPack => classPack.ClassCount)
            .ThenBy(classPack => classPack.Name)
            .ToListAsync(cancellationToken);
}
