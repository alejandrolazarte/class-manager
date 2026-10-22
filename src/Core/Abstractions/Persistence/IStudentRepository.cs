using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IStudentRepository
{
    void Add(Student student);

    Task<Student?> FindForUpdateAsync(long studentId, CancellationToken cancellationToken);

    Task<Student?> FindByClientAndNameAsync(long clientId, string fullName, CancellationToken cancellationToken);

    Task<IReadOnlyList<Student>> ListByClientAsync(long clientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Student>> ListByClientForUpdateAsync(long clientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Student>> ListAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Student>> ListByClientIdsAsync(IReadOnlyCollection<long> clientIds, CancellationToken cancellationToken);

    Task<StudentSummary?> GetSummaryByIdAsync(long studentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentSummary>> ListSummariesByIdsAsync(IReadOnlyCollection<long> studentIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentSummary>> SearchAsync(StudentSearchCriteria criteria, CancellationToken cancellationToken);
}
