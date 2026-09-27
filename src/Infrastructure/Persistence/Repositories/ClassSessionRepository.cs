namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ClassSessionRepository(AppDbContext context) : IClassSessionRepository
{
    public void Add(ClassSession session) => context.ClassSessions.Add(session);

    public Task<ClassSession?> FindForUpdateAsync(Guid classGroupId, DateOnly sessionDate, CancellationToken cancellationToken) =>
        context.ClassSessions.FirstOrDefaultAsync(
            session => session.ClassGroupId == classGroupId && session.Date == sessionDate,
            cancellationToken);

    public async Task<IReadOnlyList<ClassSession>> ListByDateAsync(DateOnly sessionDate, CancellationToken cancellationToken) =>
        await context.ClassSessions.AsNoTracking().Where(session => session.Date == sessionDate).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ClassSession>> ListBetweenAsync(DateOnly firstDate, DateOnly lastDate, CancellationToken cancellationToken) =>
        await context.ClassSessions.AsNoTracking()
            .Where(session => session.Date >= firstDate && session.Date <= lastDate)
            .ToListAsync(cancellationToken);
}
