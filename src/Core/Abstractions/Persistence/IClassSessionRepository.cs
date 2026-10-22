using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClassSessionRepository
{
    void Add(ClassSession session);

    Task<ClassSession?> FindForUpdateAsync(long classGroupId, DateOnly sessionDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassSession>> ListByDateAsync(DateOnly sessionDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassSession>> ListBetweenAsync(DateOnly firstDate, DateOnly lastDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassSession>> ListSubstitutionsAsync(
        long instructorId, DateOnly firstDate, DateOnly lastDate, CancellationToken cancellationToken);
}
