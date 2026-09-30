using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IAbsenceNoticeRepository
{
    void Add(AbsenceNotice notice);

    void Remove(AbsenceNotice notice);

    Task<AbsenceNotice?> FindForUpdateAsync(Guid classSessionId, Guid studentId, CancellationToken cancellationToken);

    Task<IReadOnlySet<Guid>> ListStudentIdsBySessionAsync(Guid classSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<NotifiedAbsence>> ListByStudentsBetweenAsync(
        IReadOnlyCollection<Guid> studentIds, DateOnly firstDate, DateOnly lastDate, CancellationToken cancellationToken);
}
