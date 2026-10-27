namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class MakeupBookingRepository(AppDbContext context) : IMakeupBookingRepository
{
    public void Add(MakeupBooking booking) => context.MakeupBookings.Add(booking);

    public void Remove(MakeupBooking booking) => context.MakeupBookings.Remove(booking);

    public Task<MakeupBooking?> FindForUpdateAsync(long classSessionId, long studentId, CancellationToken cancellationToken) =>
        context.MakeupBookings.FirstOrDefaultAsync(
            booking => booking.ClassSessionId == classSessionId && booking.StudentId == studentId,
            cancellationToken);

    public Task<IReadOnlyList<BookedStudent>> ListStudentsBySessionAsync(long classSessionId, CancellationToken cancellationToken) =>
        ListStudentsBySessionAsync(context.MakeupBookings.AsNoTracking(), classSessionId, cancellationToken);

    public Task<IReadOnlyList<BookedStudent>> ListStudentsBySessionIncludingDeletedAsync(long classSessionId, CancellationToken cancellationToken) =>
        ListStudentsBySessionAsync(context.MakeupBookings.AsNoTracking().IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter]), classSessionId, cancellationToken);

    public async Task<IReadOnlyList<MakeupBooking>> ListUnattendedForUpdateByStudentFromAsync(long studentId, DateOnly firstDate, CancellationToken cancellationToken) =>
        await (
            from booking in context.MakeupBookings
            where booking.StudentId == studentId
            join session in context.ClassSessions.AsNoTracking() on booking.ClassSessionId equals session.Id
            where session.Date >= firstDate
                && !context.Attendances.Any(attendance =>
                    attendance.ClassSessionId == booking.ClassSessionId && attendance.StudentId == booking.StudentId)
            select booking)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<long, int>> CountBySessionsAsync(
        IReadOnlyCollection<long> classSessionIds,
        CancellationToken cancellationToken) =>
        classSessionIds.Count == 0
            ? new Dictionary<long, int>()
            : await context.MakeupBookings.AsNoTracking()
                .Where(booking => classSessionIds.Contains(booking.ClassSessionId))
                .GroupBy(booking => booking.ClassSessionId)
                .Select(group => new { ClassSessionId = group.Key, Count = group.Count() })
                .ToDictionaryAsync(group => group.ClassSessionId, group => group.Count, cancellationToken);

    public async Task<IReadOnlyList<BookedClass>> ListByStudentsBetweenAsync(
        IReadOnlyCollection<long> studentIds,
        DateOnly firstDate,
        DateOnly lastDate,
        CancellationToken cancellationToken)
    {
        if (studentIds.Count == 0)
        {
            return [];
        }

        return await (
            from booking in context.MakeupBookings.AsNoTracking()
            where studentIds.Contains(booking.StudentId)
            join session in context.ClassSessions.AsNoTracking() on booking.ClassSessionId equals session.Id
            where session.Date >= firstDate && session.Date <= lastDate
            select new BookedClass(booking.StudentId, session.ClassGroupId, session.Date, session.IsCancelled))
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<BookedStudent>> ListStudentsBySessionAsync(
        IQueryable<MakeupBooking> bookings,
        long classSessionId,
        CancellationToken cancellationToken) =>
        await (
            from booking in bookings
            where booking.ClassSessionId == classSessionId
            join student in context.Students.AsNoTracking() on booking.StudentId equals student.Id
            join client in context.Clients.AsNoTracking() on student.ClientId equals client.Id
            orderby student.User.FullName
            select new BookedStudent(student.Id, student.User.FullName, student.User.BirthDate, client.Id, client.User.FullName))
            .ToListAsync(cancellationToken);
}
