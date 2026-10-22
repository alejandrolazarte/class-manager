using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IMeasurementGoalRepository
{
    void Add(MeasurementGoal goal);

    void Remove(MeasurementGoal goal);

    Task<MeasurementGoal?> FindForUpdateAsync(long studentId, long definitionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MeasurementGoal>> ListByStudentsAsync(IReadOnlyCollection<long> studentIds, CancellationToken cancellationToken);
}
