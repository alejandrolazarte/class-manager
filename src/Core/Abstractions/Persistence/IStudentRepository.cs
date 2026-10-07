using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IStudentRepository
{
    void Add(Student student);

    Task<Student?> FindForUpdateAsync(Guid studentId, CancellationToken cancellationToken);

    Task<Student?> FindByClientAndNameAsync(Guid clientId, string fullName, CancellationToken cancellationToken);

    Task<IReadOnlyList<Student>> ListByClientAsync(Guid clientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Student>> ListAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Student>> ListByClientIdsAsync(IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken);

    Task<StudentSummary?> GetSummaryByIdAsync(Guid studentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentSummary>> ListSummariesByIdsAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentSummary>> SearchAsync(StudentSearchCriteria criteria, CancellationToken cancellationToken);
}
