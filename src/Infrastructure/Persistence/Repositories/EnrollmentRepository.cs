namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class EnrollmentRepository(AppDbContext context) : IEnrollmentRepository
{
    public void Add(Enrollment enrollment) => context.Enrollments.Add(enrollment);

    public void Remove(Enrollment enrollment) => context.Enrollments.Remove(enrollment);

    public Task<Enrollment?> GetForUpdateAsync(long enrollmentId, CancellationToken cancellationToken) =>
        context.Enrollments.FirstOrDefaultAsync(enrollment => enrollment.Id == enrollmentId, cancellationToken);

    public Task<Enrollment?> FindCurrentAsync(long studentId, long classGroupId, DateOnly today, CancellationToken cancellationToken) =>
        Current(today).FirstOrDefaultAsync(
            enrollment => enrollment.StudentId == studentId && enrollment.ClassGroupId == classGroupId,
            cancellationToken);

    public Task<int> CountCurrentAsync(long classGroupId, DateOnly today, CancellationToken cancellationToken) =>
        Current(today).CountAsync(enrollment => enrollment.ClassGroupId == classGroupId, cancellationToken);

    public async Task<IReadOnlyDictionary<long, int>> CountCurrentByClassGroupAsync(DateOnly today, CancellationToken cancellationToken) =>
        await Current(today)
            .GroupBy(enrollment => enrollment.ClassGroupId)
            .Select(group => new { ClassGroupId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(group => group.ClassGroupId, group => group.Count, cancellationToken);

    public async Task<IReadOnlyList<RosterEntry>> ListRosterAsync(long classGroupId, DateOnly today, CancellationToken cancellationToken) =>
        await (
            from enrollment in Current(today)
            where enrollment.ClassGroupId == classGroupId
            join student in context.Students.AsNoTracking() on enrollment.StudentId equals student.Id
            join client in context.Clients.AsNoTracking() on student.ClientId equals client.Id
            orderby student.User.FullName
            select new RosterEntry(
                enrollment.Id,
                student.Id,
                student.User.FullName,
                student.User.BirthDate,
                client.Id,
                client.User.FullName,
                enrollment.StartDate,
                enrollment.EndDate))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RosterEntry>> ListRosterOnAsync(long classGroupId, DateOnly sessionDate, CancellationToken cancellationToken) =>
        await (
            from enrollment in ActiveOn(sessionDate)
            where enrollment.ClassGroupId == classGroupId
            join student in context.Students.AsNoTracking() on enrollment.StudentId equals student.Id
            join client in context.Clients.AsNoTracking() on student.ClientId equals client.Id
            orderby student.User.FullName
            select new RosterEntry(
                enrollment.Id,
                student.Id,
                student.User.FullName,
                student.User.BirthDate,
                client.Id,
                client.User.FullName,
                enrollment.StartDate,
                enrollment.EndDate))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<long, int>> CountActiveOnByClassGroupAsync(DateOnly sessionDate, CancellationToken cancellationToken) =>
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
            select new { client.Id, client.User.FullName, client.User.PhoneNumber, StudentFullName = student.User.FullName })
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => new EnrolledStudentInPeriod(row.Id, row.FullName, row.PhoneNumber!.Value, row.StudentFullName))];
    }

    public async Task<IReadOnlyList<Enrollment>> ListCurrentByStudentAsync(long studentId, DateOnly today, CancellationToken cancellationToken) =>
        await Current(today).Where(enrollment => enrollment.StudentId == studentId).ToListAsync(cancellationToken);

    private IQueryable<Enrollment> ActiveOn(DateOnly sessionDate) =>
        context.Enrollments.AsNoTracking().Where(enrollment =>
            enrollment.StartDate <= sessionDate && (enrollment.EndDate == null || enrollment.EndDate >= sessionDate));

    private IQueryable<Enrollment> Current(DateOnly today) =>
        context.Enrollments.AsNoTracking().Where(enrollment => enrollment.EndDate == null || enrollment.EndDate >= today);
}
