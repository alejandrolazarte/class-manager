namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class StudentPlanRepository(AppDbContext context) : IStudentPlanRepository
{
    public void Add(StudentPlanChange change) => context.StudentPlanChanges.Add(change);

    public void Remove(StudentPlanChange change) => context.StudentPlanChanges.Remove(change);

    public async Task<IReadOnlyList<StudentPlanChange>> ListByClientsAsync(IReadOnlyCollection<long> clientIds, CancellationToken cancellationToken)
    {
        if (clientIds.Count == 0)
        {
            return [];
        }

        return await context.StudentPlanChanges.AsNoTracking()
            .Include(change => change.Student)
            .ThenInclude(student => student!.User)
            .Where(change => clientIds.Contains(change.Student!.ClientId))
            .OrderBy(change => change.EffectiveFrom)
            .ToListAsync(cancellationToken);
    }

    public Task<StudentPlanChange?> FindForUpdateAsync(long studentId, DateOnly effectiveFrom, CancellationToken cancellationToken) =>
        context.StudentPlanChanges.FirstOrDefaultAsync(
            change => change.StudentId == studentId && change.EffectiveFrom == effectiveFrom,
            cancellationToken);
}
