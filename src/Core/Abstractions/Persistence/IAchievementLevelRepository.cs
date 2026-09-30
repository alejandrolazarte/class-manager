using ClassManager.Core.Domain.Achievements;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IAchievementLevelRepository
{
    Task<IReadOnlyList<LevelDefinition>> ListAsync(CancellationToken cancellationToken);

    Task ReplaceAsync(IReadOnlyList<LevelDefinition> levels, CancellationToken cancellationToken);
}
