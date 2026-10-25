using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IAttendanceRepository
{
    void Add(Attendance attendance);

    void Remove(Attendance attendance);

    Task<Attendance?> FindForUpdateAsync(long classSessionId, long studentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Attendance>> ListBySessionForUpdateAsync(long classSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Attendance>> ListBySessionAsync(long classSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, AttendanceCount>> CountBySessionsAsync(IReadOnlyCollection<long> classSessionIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientAttendedClass>> ListAttendedClassesByClientsAsync(IReadOnlyCollection<long> clientIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentAttendanceMark>> ListMarksByStudentsAsync(
        IReadOnlyCollection<long> studentIds, DateOnly firstDate, DateOnly today, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, int>> CountAttendedClassesByStudentsAsync(
        IReadOnlyCollection<long> studentIds, CancellationToken cancellationToken);
}
