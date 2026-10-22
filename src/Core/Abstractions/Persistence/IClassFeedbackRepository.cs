using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClassFeedbackRepository
{
    void Add(ClassFeedback feedback);

    void Remove(ClassFeedback feedback);

    Task<ClassFeedback?> FindForUpdateAsync(long classSessionId, long studentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassFeedback>> ListBySessionAsync(long classSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentFeedback>> ListLatestByStudentsAsync(IReadOnlyCollection<long> studentIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentFeedback>> ListUpdatedSinceAsync(
        IReadOnlyCollection<long> studentIds, DateTimeOffset since, CancellationToken cancellationToken);
}
