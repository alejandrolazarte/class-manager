using ClassManager.Core.Domain.Enrollments;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IEnrollmentRepository
{
    void Add(Enrollment enrollment);

    void Remove(Enrollment enrollment);

    Task<Enrollment?> GetForUpdateAsync(Guid enrollmentId, CancellationToken cancellationToken);

    Task<Enrollment?> FindCurrentAsync(Guid studentId, Guid classGroupId, DateOnly today, CancellationToken cancellationToken);

    Task<int> CountCurrentAsync(Guid classGroupId, DateOnly today, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, int>> CountCurrentByClassGroupAsync(DateOnly today, CancellationToken cancellationToken);

    Task<IReadOnlyList<RosterEntry>> ListRosterAsync(Guid classGroupId, DateOnly today, CancellationToken cancellationToken);

    Task<IReadOnlyList<Enrollment>> ListCurrentByStudentAsync(Guid studentId, DateOnly today, CancellationToken cancellationToken);
}
