namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class PackBookingRepository(AppDbContext context) : IPackBookingRepository
{
    public void Add(PackBooking booking) => context.PackBookings.Add(booking);

    public void Remove(PackBooking booking) => context.PackBookings.Remove(booking);

    public Task<PackBooking?> FindForUpdateAsync(Guid classSessionId, Guid studentId, CancellationToken cancellationToken) =>
        context.PackBookings.FirstOrDefaultAsync(
            booking => booking.ClassSessionId == classSessionId && booking.StudentId == studentId,
            cancellationToken);

    public async Task<IReadOnlyList<BookedStudent>> ListStudentsBySessionAsync(Guid classSessionId, CancellationToken cancellationToken) =>
        await (
            from booking in context.PackBookings.AsNoTracking()
            where booking.ClassSessionId == classSessionId
            join student in context.Students.AsNoTracking() on booking.StudentId equals student.Id
            join client in context.Clients.AsNoTracking() on student.ClientId equals client.Id
            orderby student.FullName
            select new BookedStudent(student.Id, student.FullName, student.BirthDate, client.FullName))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<Guid, int>> CountBySessionsAsync(
        IReadOnlyCollection<Guid> classSessionIds,
        CancellationToken cancellationToken) =>
        classSessionIds.Count == 0
            ? new Dictionary<Guid, int>()
            : await context.PackBookings.AsNoTracking()
                .Where(booking => classSessionIds.Contains(booking.ClassSessionId))
                .GroupBy(booking => booking.ClassSessionId)
                .Select(group => new { ClassSessionId = group.Key, Count = group.Count() })
                .ToDictionaryAsync(group => group.ClassSessionId, group => group.Count, cancellationToken);

    public async Task<IReadOnlyList<BookedClass>> ListByStudentsBetweenAsync(
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
            from booking in context.PackBookings.AsNoTracking()
            where studentIds.Contains(booking.StudentId)
            join session in context.ClassSessions.AsNoTracking() on booking.ClassSessionId equals session.Id
            where session.Date >= firstDate && session.Date <= lastDate
            select new BookedClass(booking.StudentId, session.ClassGroupId, session.Date, session.IsCancelled))
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountWaitingForAttendanceAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken) =>
        studentIds.Count == 0
            ? Task.FromResult(0)
            : (
                from booking in context.PackBookings.AsNoTracking()
                where studentIds.Contains(booking.StudentId)
                join session in context.ClassSessions.AsNoTracking() on booking.ClassSessionId equals session.Id
                where !session.IsCancelled
                    && !context.Attendances.Any(attendance =>
                        attendance.ClassSessionId == booking.ClassSessionId && attendance.StudentId == booking.StudentId)
                select booking.Id)
                .CountAsync(cancellationToken);
}
