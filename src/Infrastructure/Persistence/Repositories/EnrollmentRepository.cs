namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class EnrollmentRepository(AppDbContext context) : IEnrollmentRepository
{
    public void Add(Enrollment enrollment) => context.Enrollments.Add(enrollment);

    public void Remove(Enrollment enrollment) => context.Enrollments.Remove(enrollment);

    public Task<Enrollment?> GetForUpdateAsync(Guid enrollmentId, CancellationToken cancellationToken) =>
        context.Enrollments.FirstOrDefaultAsync(enrollment => enrollment.Id == enrollmentId, cancellationToken);

    public Task<Enrollment?> FindCurrentAsync(Guid studentId, Guid classGroupId, DateOnly today, CancellationToken cancellationToken) =>
        Current(today).FirstOrDefaultAsync(
            enrollment => enrollment.StudentId == studentId && enrollment.ClassGroupId == classGroupId,
            cancellationToken);

    public Task<int> CountCurrentAsync(Guid classGroupId, DateOnly today, CancellationToken cancellationToken) =>
        Current(today).CountAsync(enrollment => enrollment.ClassGroupId == classGroupId, cancellationToken);

    public async Task<IReadOnlyDictionary<Guid, int>> CountCurrentByClassGroupAsync(DateOnly today, CancellationToken cancellationToken) =>
        await Current(today)
            .GroupBy(enrollment => enrollment.ClassGroupId)
            .Select(group => new { ClassGroupId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(group => group.ClassGroupId, group => group.Count, cancellationToken);

    public async Task<IReadOnlyList<RosterEntry>> ListRosterAsync(Guid classGroupId, DateOnly today, CancellationToken cancellationToken) =>
        await (
            from enrollment in Current(today)
            where enrollment.ClassGroupId == classGroupId
            join student in context.Students.AsNoTracking() on enrollment.StudentId equals student.Id
            join client in context.Clients.AsNoTracking() on student.ClientId equals client.Id
            orderby student.FullName
            select new RosterEntry(
                enrollment.Id,
                student.Id,
                student.FullName,
                student.BirthDate,
                client.Id,
                client.FullName,
                enrollment.StartDate,
                enrollment.EndDate))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RosterEntry>> ListRosterOnAsync(Guid classGroupId, DateOnly sessionDate, CancellationToken cancellationToken) =>
        await (
            from enrollment in ActiveOn(sessionDate)
            where enrollment.ClassGroupId == classGroupId
            join student in context.Students.AsNoTracking() on enrollment.StudentId equals student.Id
            join client in context.Clients.AsNoTracking() on student.ClientId equals client.Id
            orderby student.FullName
            select new RosterEntry(
                enrollment.Id,
                student.Id,
                student.FullName,
                student.BirthDate,
                client.Id,
                client.FullName,
                enrollment.StartDate,
                enrollment.EndDate))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<Guid, int>> CountActiveOnByClassGroupAsync(DateOnly sessionDate, CancellationToken cancellationToken) =>
        await ActiveOn(sessionDate)
            .GroupBy(enrollment => enrollment.ClassGroupId)
            .Select(group => new { ClassGroupId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(group => group.ClassGroupId, group => group.Count, cancellationToken);

    public async Task<IReadOnlyList<ClassGroupEnrollmentPeriod>> ListActiveInPeriodAsync(
        DateOnly periodStart,
        DateOnly periodEnd,
        CancellationToken cancellationToken) =>
        await context.Enrollments.AsNoTracking()
            .Where(enrollment => enrollment.StartDate <= periodEnd && (enrollment.EndDate == null || enrollment.EndDate >= periodStart))
            .Select(enrollment => new ClassGroupEnrollmentPeriod(enrollment.ClassGroupId, enrollment.StartDate, enrollment.EndDate))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<EnrolledStudentInPeriod>> ListEnrolledInPeriodAsync(
        DateOnly periodStart,
        DateOnly periodEnd,
        CancellationToken cancellationToken)
    {
        var rows = await (
            from enrollment in context.Enrollments.AsNoTracking()
            where enrollment.StartDate <= periodEnd && (enrollment.EndDate == null || enrollment.EndDate >= periodStart)
            join student in context.Students.AsNoTracking() on enrollment.StudentId equals student.Id
            join client in context.Clients.AsNoTracking() on student.ClientId equals client.Id
            select new { client.Id, client.FullName, client.PhoneNumber, StudentFullName = student.FullName })
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => new EnrolledStudentInPeriod(row.Id, row.FullName, row.PhoneNumber.Value, row.StudentFullName))];
    }

    public async Task<IReadOnlyList<Enrollment>> ListCurrentByStudentAsync(Guid studentId, DateOnly today, CancellationToken cancellationToken) =>
        await Current(today).Where(enrollment => enrollment.StudentId == studentId).ToListAsync(cancellationToken);

    private IQueryable<Enrollment> ActiveOn(DateOnly sessionDate) =>
        context.Enrollments.AsNoTracking().Where(enrollment =>
            enrollment.StartDate <= sessionDate && (enrollment.EndDate == null || enrollment.EndDate >= sessionDate));

    private IQueryable<Enrollment> Current(DateOnly today) =>
        context.Enrollments.AsNoTracking().Where(enrollment => enrollment.EndDate == null || enrollment.EndDate >= today);
}
