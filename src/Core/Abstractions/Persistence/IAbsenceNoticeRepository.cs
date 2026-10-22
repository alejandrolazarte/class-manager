using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IAbsenceNoticeRepository
{
    void Add(AbsenceNotice notice);

    void Remove(AbsenceNotice notice);

    Task<AbsenceNotice?> FindForUpdateAsync(long classSessionId, long studentId, CancellationToken cancellationToken);

    Task<IReadOnlySet<long>> ListStudentIdsBySessionAsync(long classSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, int>> CountBySessionsAsync(IReadOnlyCollection<long> classSessionIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<NotifiedAbsence>> ListByStudentsBetweenAsync(
        IReadOnlyCollection<long> studentIds, DateOnly firstDate, DateOnly lastDate, CancellationToken cancellationToken);
}
