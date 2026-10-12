namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class MeasurementRecordRepository(AppDbContext context) : IMeasurementRecordRepository
{
    public void Add(MeasurementRecord record) => context.MeasurementRecords.Add(record);

    public void Remove(MeasurementRecord record) => context.MeasurementRecords.Remove(record);

    public Task<MeasurementRecord?> FindForUpdateAsync(Guid recordId, CancellationToken cancellationToken) =>
        context.MeasurementRecords.FirstOrDefaultAsync(record => record.Id == recordId, cancellationToken);

    public Task<MeasurementRecord?> FindDeletedForUpdateAsync(Guid recordId, CancellationToken cancellationToken) =>
        context.MeasurementRecords
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .FirstOrDefaultAsync(record => record.Id == recordId && record.IsDeleted, cancellationToken);

    public Task<MeasurementRecord?> FindOnDateForUpdateAsync(Guid studentId, Guid definitionId, DateOnly measuredOn, CancellationToken cancellationToken) =>
        context.MeasurementRecords.FirstOrDefaultAsync(
            record => record.StudentId == studentId && record.MeasurementDefinitionId == definitionId && record.MeasuredOn == measuredOn,
            cancellationToken);

    public async Task<IReadOnlyList<MeasurementRecord>> ListByStudentsAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken) =>
        await context.MeasurementRecords.AsNoTracking()
            .Where(record => studentIds.Contains(record.StudentId))
            .OrderBy(record => record.MeasuredOn)
            .ToListAsync(cancellationToken);
}
