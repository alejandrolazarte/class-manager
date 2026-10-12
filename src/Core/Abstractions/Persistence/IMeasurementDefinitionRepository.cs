using ClassManager.Core.Domain.Measurements;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IMeasurementDefinitionRepository
{
    void Add(MeasurementDefinition definition);

    void Remove(MeasurementDefinition definition);

    Task<MeasurementDefinition?> FindForUpdateAsync(Guid definitionId, CancellationToken cancellationToken);

    Task<MeasurementDefinition?> FindDeletedForUpdateAsync(Guid definitionId, CancellationToken cancellationToken);

    Task<MeasurementDefinition?> GetAsync(Guid definitionId, CancellationToken cancellationToken);

    Task<bool> NameExistsAsync(string name, Guid? exceptDefinitionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MeasurementDefinition>> ListAsync(CancellationToken cancellationToken);
}
