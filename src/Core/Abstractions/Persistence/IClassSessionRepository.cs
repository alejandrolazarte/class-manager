using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClassSessionRepository
{
    void Add(ClassSession session);

    Task<ClassSession?> FindForUpdateAsync(Guid classGroupId, DateOnly sessionDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassSession>> ListByDateAsync(DateOnly sessionDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassSession>> ListBetweenAsync(DateOnly firstDate, DateOnly lastDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassSession>> ListSubstitutionsAsync(
        Guid instructorId, DateOnly firstDate, DateOnly lastDate, CancellationToken cancellationToken);
}
