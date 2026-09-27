namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class AttendanceRepository(AppDbContext context) : IAttendanceRepository
{
    public void Add(Attendance attendance) => context.Attendances.Add(attendance);

    public void Remove(Attendance attendance) => context.Attendances.Remove(attendance);

    public Task<Attendance?> FindForUpdateAsync(Guid classSessionId, Guid studentId, CancellationToken cancellationToken) =>
        context.Attendances.FirstOrDefaultAsync(
            attendance => attendance.ClassSessionId == classSessionId && attendance.StudentId == studentId,
            cancellationToken);

    public async Task<IReadOnlyList<Attendance>> ListBySessionAsync(Guid classSessionId, CancellationToken cancellationToken) =>
        await context.Attendances.AsNoTracking()
            .Where(attendance => attendance.ClassSessionId == classSessionId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<Guid, AttendanceCount>> CountBySessionsAsync(
        IReadOnlyCollection<Guid> classSessionIds,
        CancellationToken cancellationToken)
    {
        if (classSessionIds.Count == 0)
        {
            return new Dictionary<Guid, AttendanceCount>();
        }

        var counts = await context.Attendances.AsNoTracking()
            .Where(attendance => classSessionIds.Contains(attendance.ClassSessionId))
            .GroupBy(attendance => attendance.ClassSessionId)
            .Select(group => new
            {
                ClassSessionId = group.Key,
                Present = group.Count(attendance => attendance.Status == AttendanceStatus.Present),
                Absent = group.Count(attendance => attendance.Status == AttendanceStatus.Absent),
            })
            .ToListAsync(cancellationToken);

        return counts.ToDictionary(count => count.ClassSessionId, count => new AttendanceCount(count.Present, count.Absent));
    }

    public async Task<IReadOnlyList<ClientAttendedClass>> ListAttendedClassesByClientsAsync(
        IReadOnlyCollection<Guid> clientIds,
        CancellationToken cancellationToken)
    {
        if (clientIds.Count == 0)
        {
            return [];
        }

        var rows = await (
            from attendance in context.Attendances.AsNoTracking()
            where attendance.Status == AttendanceStatus.Present
            join student in context.Students.AsNoTracking() on attendance.StudentId equals student.Id
            where clientIds.Contains(student.ClientId)
            join session in context.ClassSessions.AsNoTracking() on attendance.ClassSessionId equals session.Id
            join classGroup in context.ClassGroups.AsNoTracking() on session.ClassGroupId equals classGroup.Id
            select new { student.ClientId, session.Date, StudentFullName = student.FullName, ClassGroupName = classGroup.Name })
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => new ClientAttendedClass(row.ClientId, new AttendedClass(row.Date, row.StudentFullName, row.ClassGroupName)))];
    }
}
