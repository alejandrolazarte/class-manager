using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClassGroupRepository
{
    void Add(ClassGroup classGroup);

    Task<ClassGroup?> GetByIdAsync(long classGroupId, CancellationToken cancellationToken);

    Task<ClassGroup?> GetForUpdateAsync(long classGroupId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassGroup>> ListAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassGroup>> ListActiveAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassGroup>> ListByIdsAsync(IReadOnlyCollection<long> classGroupIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassGroup>> ListActiveByInstructorAsync(long instructorId, CancellationToken cancellationToken);

    Task<int> CountActiveByInstructorAsync(long instructorId, CancellationToken cancellationToken);
}
