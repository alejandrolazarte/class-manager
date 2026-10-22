using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IMeasurementDefinitionRepository
{
    void Add(MeasurementDefinition definition);

    void Remove(MeasurementDefinition definition);

    Task<MeasurementDefinition?> FindForUpdateAsync(long definitionId, CancellationToken cancellationToken);

    Task<MeasurementDefinition?> FindDeletedForUpdateAsync(long definitionId, CancellationToken cancellationToken);

    Task<MeasurementDefinition?> GetAsync(long definitionId, CancellationToken cancellationToken);

    Task<bool> NameExistsAsync(string name, long? exceptDefinitionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MeasurementDefinition>> ListAsync(CancellationToken cancellationToken);
}
