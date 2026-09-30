namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class AbsenceNoticeRepository(AppDbContext context) : IAbsenceNoticeRepository
{
    public void Add(AbsenceNotice notice) => context.AbsenceNotices.Add(notice);

    public void Remove(AbsenceNotice notice) => context.AbsenceNotices.Remove(notice);

    public Task<AbsenceNotice?> FindForUpdateAsync(Guid classSessionId, Guid studentId, CancellationToken cancellationToken) =>
        context.AbsenceNotices.FirstOrDefaultAsync(
            notice => notice.ClassSessionId == classSessionId && notice.StudentId == studentId,
            cancellationToken);

    public async Task<IReadOnlySet<Guid>> ListStudentIdsBySessionAsync(Guid classSessionId, CancellationToken cancellationToken) =>
        (await context.AbsenceNotices.AsNoTracking()
            .Where(notice => notice.ClassSessionId == classSessionId)
            .Select(notice => notice.StudentId)
            .ToListAsync(cancellationToken))
            .ToHashSet();

    public async Task<IReadOnlyDictionary<Guid, int>> CountBySessionsAsync(
        IReadOnlyCollection<Guid> classSessionIds,
        CancellationToken cancellationToken) =>
        classSessionIds.Count == 0
            ? new Dictionary<Guid, int>()
            : await context.AbsenceNotices.AsNoTracking()
                .Where(notice => classSessionIds.Contains(notice.ClassSessionId))
                .GroupBy(notice => notice.ClassSessionId)
                .Select(group => new { ClassSessionId = group.Key, Count = group.Count() })
                .ToDictionaryAsync(group => group.ClassSessionId, group => group.Count, cancellationToken);

    public async Task<IReadOnlyList<NotifiedAbsence>> ListByStudentsBetweenAsync(
        IReadOnlyCollection<Guid> studentIds,
        DateOnly firstDate,
        DateOnly lastDate,
        CancellationToken cancellationToken)
    {
        if (studentIds.Count == 0)
        {
            return [];
        }

        return await (
            from notice in context.AbsenceNotices.AsNoTracking()
            where studentIds.Contains(notice.StudentId)
            join session in context.ClassSessions.AsNoTracking() on notice.ClassSessionId equals session.Id
            where session.Date >= firstDate && session.Date <= lastDate
            select new NotifiedAbsence(notice.StudentId, session.ClassGroupId, session.Date))
            .ToListAsync(cancellationToken);
    }
}
