namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class CustomRoleRepository(AppDbContext context) : ICustomRoleRepository
{
    public void Add(CustomRole role) => context.CustomRoles.Add(role);

    public void Remove(CustomRole role) => context.CustomRoles.Remove(role);

    public Task<CustomRole?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken) =>
        context.CustomRoles.AsNoTracking().FirstOrDefaultAsync(role => role.Id == roleId, cancellationToken);

    public Task<CustomRole?> GetForUpdateAsync(Guid roleId, CancellationToken cancellationToken) =>
        context.CustomRoles.FirstOrDefaultAsync(role => role.Id == roleId, cancellationToken);

    public async Task<IReadOnlyList<CustomRole>> ListAsync(CancellationToken cancellationToken) =>
        await context.CustomRoles.AsNoTracking().OrderBy(role => role.Name).ToListAsync(cancellationToken);
}
