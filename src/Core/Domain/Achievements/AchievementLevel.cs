using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Achievements;

public sealed class AchievementLevel : ITenantOwned, IAuditable
{
    private AchievementLevel()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public int Position { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int RequiredClasses { get; private set; }

    public static AchievementLevel Create(int position, LevelDefinition definition) =>
        new()
        {
            Position = position,
            Name = definition.Name.Trim(),
            RequiredClasses = definition.RequiredClasses,
        };

    public LevelDefinition ToDefinition() => new(Name, RequiredClasses);
}
