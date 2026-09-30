namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class MakeupBookingRepository(AppDbContext context) : IMakeupBookingRepository
{
    public void Add(MakeupBooking booking) => context.MakeupBookings.Add(booking);

    public void Remove(MakeupBooking booking) => context.MakeupBookings.Remove(booking);

    public Task<MakeupBooking?> FindForUpdateAsync(Guid classSessionId, Guid studentId, CancellationToken cancellationToken) =>
        context.MakeupBookings.FirstOrDefaultAsync(
            booking => booking.ClassSessionId == classSessionId && booking.StudentId == studentId,
            cancellationToken);

    public async Task<IReadOnlyList<MakeupStudent>> ListStudentsBySessionAsync(Guid classSessionId, CancellationToken cancellationToken) =>
        await (
            from booking in context.MakeupBookings.AsNoTracking()
            where booking.ClassSessionId == classSessionId
            join student in context.Students.AsNoTracking() on booking.StudentId equals student.Id
            join client in context.Clients.AsNoTracking() on student.ClientId equals client.Id
            orderby student.FullName
            select new MakeupStudent(student.Id, student.FullName, student.BirthDate, client.FullName))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<Guid, int>> CountBySessionsAsync(
        IReadOnlyCollection<Guid> classSessionIds,
        CancellationToken cancellationToken) =>
        classSessionIds.Count == 0
            ? new Dictionary<Guid, int>()
            : await context.MakeupBookings.AsNoTracking()
                .Where(booking => classSessionIds.Contains(booking.ClassSessionId))
                .GroupBy(booking => booking.ClassSessionId)
                .Select(group => new { ClassSessionId = group.Key, Count = group.Count() })
                .ToDictionaryAsync(group => group.ClassSessionId, group => group.Count, cancellationToken);

    public async Task<IReadOnlyList<BookedMakeup>> ListByStudentsBetweenAsync(
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
            from booking in context.MakeupBookings.AsNoTracking()
            where studentIds.Contains(booking.StudentId)
            join session in context.ClassSessions.AsNoTracking() on booking.ClassSessionId equals session.Id
            where session.Date >= firstDate && session.Date <= lastDate
            select new BookedMakeup(booking.StudentId, session.ClassGroupId, session.Date, session.IsCancelled))
            .ToListAsync(cancellationToken);
    }
}
