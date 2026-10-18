using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IMeasurementRecordRepository
{
    void Add(MeasurementRecord record);

    void Remove(MeasurementRecord record);

    Task<MeasurementRecord?> FindForUpdateAsync(Guid recordId, CancellationToken cancellationToken);

    Task<MeasurementRecord?> FindDeletedForUpdateAsync(Guid recordId, CancellationToken cancellationToken);

    Task<MeasurementRecord?> FindOnDateForUpdateAsync(Guid studentId, Guid definitionId, DateOnly measuredOn, CancellationToken cancellationToken);

    Task<IReadOnlyList<MeasurementRecord>> ListByStudentsAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<MeasurementRecord>> ListOnDateAsync(IReadOnlyCollection<Guid> studentIds, DateOnly measuredOn, CancellationToken cancellationToken);
}
