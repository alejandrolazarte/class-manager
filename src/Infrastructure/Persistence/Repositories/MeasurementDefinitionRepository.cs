namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class MeasurementDefinitionRepository(AppDbContext context) : IMeasurementDefinitionRepository
{
    public void Add(MeasurementDefinition definition) => context.MeasurementDefinitions.Add(definition);

    public void Remove(MeasurementDefinition definition) => context.MeasurementDefinitions.Remove(definition);

    public Task<MeasurementDefinition?> FindForUpdateAsync(Guid definitionId, CancellationToken cancellationToken) =>
        context.MeasurementDefinitions.FirstOrDefaultAsync(definition => definition.Id == definitionId, cancellationToken);

    public Task<MeasurementDefinition?> FindDeletedForUpdateAsync(Guid definitionId, CancellationToken cancellationToken) =>
        context.MeasurementDefinitions
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .FirstOrDefaultAsync(definition => definition.Id == definitionId && definition.IsDeleted, cancellationToken);

    public Task<MeasurementDefinition?> GetAsync(Guid definitionId, CancellationToken cancellationToken) =>
        context.MeasurementDefinitions.AsNoTracking().FirstOrDefaultAsync(definition => definition.Id == definitionId, cancellationToken);

    public Task<bool> NameExistsAsync(string name, Guid? exceptDefinitionId, CancellationToken cancellationToken) =>
        context.MeasurementDefinitions.AnyAsync(
            definition => definition.Name == name && definition.Id != exceptDefinitionId,
            cancellationToken);

    public async Task<IReadOnlyList<MeasurementDefinition>> ListAsync(CancellationToken cancellationToken) =>
        await context.MeasurementDefinitions.AsNoTracking()
            .OrderBy(definition => definition.CreatedAt)
            .ToListAsync(cancellationToken);
}
