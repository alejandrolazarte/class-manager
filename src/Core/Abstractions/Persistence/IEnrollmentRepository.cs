using ClassManager.Core.Domain.Enrollments;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IEnrollmentRepository
{
    void Add(Enrollment enrollment);

    void Remove(Enrollment enrollment);

    Task<Enrollment?> GetForUpdateAsync(long enrollmentId, CancellationToken cancellationToken);

    Task<Enrollment?> FindCurrentAsync(long studentId, long classGroupId, DateOnly today, CancellationToken cancellationToken);

    Task<int> CountCurrentAsync(long classGroupId, DateOnly today, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, int>> CountCurrentByClassGroupAsync(DateOnly today, CancellationToken cancellationToken);

    Task<IReadOnlyList<RosterEntry>> ListRosterAsync(long classGroupId, DateOnly today, CancellationToken cancellationToken);

    Task<IReadOnlyList<RosterEntry>> ListRosterOnAsync(long classGroupId, DateOnly sessionDate, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, int>> CountActiveOnByClassGroupAsync(DateOnly sessionDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassGroupEnrollmentPeriod>> ListActiveInPeriodAsync(DateOnly periodStart, DateOnly periodEnd, CancellationToken cancellationToken);

    Task<IReadOnlyList<EnrolledStudentInPeriod>> ListEnrolledInPeriodAsync(DateOnly periodStart, DateOnly periodEnd, CancellationToken cancellationToken);

    Task<IReadOnlyList<Enrollment>> ListOpenByStudentForUpdateAsync(long studentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Enrollment>> ListCurrentByStudentAsync(long studentId, DateOnly today, CancellationToken cancellationToken);

    Task<IReadOnlyList<Enrollment>> ListCurrentByStudentsAsync(IReadOnlyCollection<long> studentIds, DateOnly today, CancellationToken cancellationToken);
}
