using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IPackBookingRepository
{
    void Add(PackBooking booking);

    void Remove(PackBooking booking);

    Task<PackBooking?> FindForUpdateAsync(long classSessionId, long studentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<BookedStudent>> ListStudentsBySessionAsync(long classSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<BookedStudent>> ListStudentsBySessionIncludingDeletedAsync(long classSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<PackBooking>> ListUnattendedForUpdateByStudentFromAsync(long studentId, DateOnly firstDate, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, int>> CountBySessionsAsync(IReadOnlyCollection<long> classSessionIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<BookedClass>> ListByStudentsBetweenAsync(
        IReadOnlyCollection<long> studentIds, DateOnly firstDate, DateOnly lastDate, CancellationToken cancellationToken);

    Task<int> CountWaitingForAttendanceAsync(IReadOnlyCollection<long> studentIds, CancellationToken cancellationToken);
}
