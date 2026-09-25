using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClassGroupRepository
{
    void Add(ClassGroup classGroup);

    Task<ClassGroup?> GetByIdAsync(Guid classGroupId, CancellationToken cancellationToken);

    Task<ClassGroup?> GetForUpdateAsync(Guid classGroupId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassGroup>> ListAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassGroup>> ListActiveAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassGroup>> ListActiveByInstructorAsync(Guid instructorId, CancellationToken cancellationToken);

    Task<int> CountActiveByInstructorAsync(Guid instructorId, CancellationToken cancellationToken);
}
