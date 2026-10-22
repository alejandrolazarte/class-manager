namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class MeasurementRecordRepository(AppDbContext context) : IMeasurementRecordRepository
{
    public void Add(MeasurementRecord record) => context.MeasurementRecords.Add(record);

    public void Remove(MeasurementRecord record) => context.MeasurementRecords.Remove(record);

    public Task<MeasurementRecord?> FindForUpdateAsync(long recordId, CancellationToken cancellationToken) =>
        context.MeasurementRecords.FirstOrDefaultAsync(record => record.Id == recordId, cancellationToken);

    public Task<MeasurementRecord?> FindDeletedForUpdateAsync(long recordId, CancellationToken cancellationToken) =>
        context.MeasurementRecords
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .FirstOrDefaultAsync(record => record.Id == recordId && record.IsDeleted, cancellationToken);

    public Task<MeasurementRecord?> FindOnDateForUpdateAsync(long studentId, long definitionId, DateOnly measuredOn, CancellationToken cancellationToken) =>
        context.MeasurementRecords.FirstOrDefaultAsync(
            record => record.StudentId == studentId && record.MeasurementDefinitionId == definitionId && record.MeasuredOn == measuredOn,
            cancellationToken);

    public async Task<IReadOnlyList<MeasurementRecord>> ListByStudentsAsync(IReadOnlyCollection<long> studentIds, CancellationToken cancellationToken) =>
        await context.MeasurementRecords.AsNoTracking()
            .Where(record => studentIds.Contains(record.StudentId))
            .OrderBy(record => record.MeasuredOn)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<MeasurementRecord>> ListOnDateAsync(IReadOnlyCollection<long> studentIds, DateOnly measuredOn, CancellationToken cancellationToken) =>
        await context.MeasurementRecords.AsNoTracking()
            .Where(record => studentIds.Contains(record.StudentId) && record.MeasuredOn == measuredOn)
            .ToListAsync(cancellationToken);
}
