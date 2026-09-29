using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IAttendanceRepository
{
    void Add(Attendance attendance);

    void Remove(Attendance attendance);

    Task<Attendance?> FindForUpdateAsync(Guid classSessionId, Guid studentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Attendance>> ListBySessionAsync(Guid classSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, AttendanceCount>> CountBySessionsAsync(IReadOnlyCollection<Guid> classSessionIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientAttendedClass>> ListAttendedClassesByClientsAsync(IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentAttendanceMark>> ListMarksByStudentsAsync(
        IReadOnlyCollection<Guid> studentIds, DateOnly firstDate, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, int>> CountAttendedClassesByStudentsAsync(
        IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken);
}
