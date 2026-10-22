using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IMeasurementRecordRepository
{
    void Add(MeasurementRecord record);

    void Remove(MeasurementRecord record);

    Task<MeasurementRecord?> FindForUpdateAsync(long recordId, CancellationToken cancellationToken);

    Task<MeasurementRecord?> FindDeletedForUpdateAsync(long recordId, CancellationToken cancellationToken);

    Task<MeasurementRecord?> FindOnDateForUpdateAsync(long studentId, long definitionId, DateOnly measuredOn, CancellationToken cancellationToken);

    Task<IReadOnlyList<MeasurementRecord>> ListByStudentsAsync(IReadOnlyCollection<long> studentIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<MeasurementRecord>> ListOnDateAsync(IReadOnlyCollection<long> studentIds, DateOnly measuredOn, CancellationToken cancellationToken);
}
