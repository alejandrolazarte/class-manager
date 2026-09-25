namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ClassGroupRepository(AppDbContext context) : IClassGroupRepository
{
    public void Add(ClassGroup classGroup) => context.ClassGroups.Add(classGroup);

    public Task<ClassGroup?> GetByIdAsync(Guid classGroupId, CancellationToken cancellationToken) =>
        context.ClassGroups.AsNoTracking().FirstOrDefaultAsync(classGroup => classGroup.Id == classGroupId, cancellationToken);

    public Task<ClassGroup?> GetForUpdateAsync(Guid classGroupId, CancellationToken cancellationToken) =>
        context.ClassGroups.FirstOrDefaultAsync(classGroup => classGroup.Id == classGroupId, cancellationToken);

    public async Task<IReadOnlyList<ClassGroup>> ListAllAsync(CancellationToken cancellationToken) =>
        await context.ClassGroups.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ClassGroup>> ListActiveAsync(CancellationToken cancellationToken) =>
        await context.ClassGroups.AsNoTracking().Where(classGroup => classGroup.IsActive).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ClassGroup>> ListActiveByInstructorAsync(Guid instructorId, CancellationToken cancellationToken) =>
        await context.ClassGroups.AsNoTracking()
            .Where(classGroup => classGroup.IsActive && classGroup.InstructorId == instructorId)
            .ToListAsync(cancellationToken);

    public Task<int> CountActiveByInstructorAsync(Guid instructorId, CancellationToken cancellationToken) =>
        context.ClassGroups.CountAsync(classGroup => classGroup.IsActive && classGroup.InstructorId == instructorId, cancellationToken);
}
