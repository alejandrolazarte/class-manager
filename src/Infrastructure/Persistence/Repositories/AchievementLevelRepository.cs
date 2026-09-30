namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class AchievementLevelRepository(AppDbContext context) : IAchievementLevelRepository
{
    public async Task<IReadOnlyList<LevelDefinition>> ListAsync(CancellationToken cancellationToken)
    {
        var levels = await context.AchievementLevels.AsNoTracking()
            .OrderBy(level => level.Position)
            .Select(level => new LevelDefinition(level.Name, level.RequiredClasses))
            .ToListAsync(cancellationToken);
        return levels.Count == 0 ? LevelLadder.Defaults : levels;
    }

    public async Task ReplaceAsync(IReadOnlyList<LevelDefinition> levels, CancellationToken cancellationToken)
    {
        context.AchievementLevels.RemoveRange(await context.AchievementLevels.ToListAsync(cancellationToken));
        context.AchievementLevels.AddRange(levels.Select((level, position) => AchievementLevel.Create(position, level)));
    }
}
