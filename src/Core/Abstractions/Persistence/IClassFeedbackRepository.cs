using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClassFeedbackRepository
{
    void Add(ClassFeedback feedback);

    void Remove(ClassFeedback feedback);

    Task<ClassFeedback?> FindForUpdateAsync(Guid classSessionId, Guid studentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassFeedback>> ListBySessionAsync(Guid classSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentFeedback>> ListLatestByStudentsAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken);
}
