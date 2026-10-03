using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IMakeupBookingRepository
{
    void Add(MakeupBooking booking);

    void Remove(MakeupBooking booking);

    Task<MakeupBooking?> FindForUpdateAsync(Guid classSessionId, Guid studentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<BookedStudent>> ListStudentsBySessionAsync(Guid classSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, int>> CountBySessionsAsync(IReadOnlyCollection<Guid> classSessionIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<BookedClass>> ListByStudentsBetweenAsync(
        IReadOnlyCollection<Guid> studentIds, DateOnly firstDate, DateOnly lastDate, CancellationToken cancellationToken);
}
