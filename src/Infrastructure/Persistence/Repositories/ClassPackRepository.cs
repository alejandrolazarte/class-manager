namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ClassPackRepository(AppDbContext context) : IClassPackRepository
{
    public void Add(ClassPack classPack) => context.ClassPacks.Add(classPack);

    public Task<ClassPack?> GetByIdAsync(Guid classPackId, CancellationToken cancellationToken) =>
        context.ClassPacks.AsNoTracking().FirstOrDefaultAsync(classPack => classPack.Id == classPackId, cancellationToken);

    public Task<ClassPack?> GetForUpdateAsync(Guid classPackId, CancellationToken cancellationToken) =>
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
