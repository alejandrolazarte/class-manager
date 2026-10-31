using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IStudentPlanRepository
{
    void Add(StudentPlanChange change);

    void Remove(StudentPlanChange change);

    Task<IReadOnlyList<StudentPlanChange>> ListByClientsAsync(IReadOnlyCollection<long> clientIds, CancellationToken cancellationToken);

    Task<StudentPlanChange?> FindForUpdateAsync(long studentId, DateOnly effectiveFrom, CancellationToken cancellationToken);
}
