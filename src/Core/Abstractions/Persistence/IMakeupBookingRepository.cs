using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IMakeupBookingRepository
{
    void Add(MakeupBooking booking);

    void Remove(MakeupBooking booking);

    Task<MakeupBooking?> FindForUpdateAsync(long classSessionId, long studentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<BookedStudent>> ListStudentsBySessionAsync(long classSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<BookedStudent>> ListStudentsBySessionIncludingDeletedAsync(long classSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MakeupBooking>> ListUnattendedForUpdateByStudentFromAsync(long studentId, DateOnly firstDate, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, int>> CountBySessionsAsync(IReadOnlyCollection<long> classSessionIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<BookedClass>> ListByStudentsBetweenAsync(
        IReadOnlyCollection<long> studentIds, DateOnly firstDate, DateOnly lastDate, CancellationToken cancellationToken);
}
