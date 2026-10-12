using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IMeasurementGoalRepository
{
    void Add(MeasurementGoal goal);

    void Remove(MeasurementGoal goal);

    Task<MeasurementGoal?> FindForUpdateAsync(Guid studentId, Guid definitionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MeasurementGoal>> ListByStudentsAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken);
}
