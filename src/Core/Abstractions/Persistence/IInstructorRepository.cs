using ClassManager.Core.Domain.Instructors;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IInstructorRepository
{
    void Add(Instructor instructor);

    Task<Instructor?> GetByIdAsync(Guid instructorId, CancellationToken cancellationToken);

    Task<Instructor?> GetForUpdateAsync(Guid instructorId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Instructor>> ListForUpdateInAnyBusinessByMemberUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<Instructor?> FindByNameAsync(string fullName, CancellationToken cancellationToken);

    Task<IReadOnlyList<Instructor>> ListAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Instructor>> ListActiveAsync(CancellationToken cancellationToken);
}
