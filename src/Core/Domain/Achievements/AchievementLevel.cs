using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Achievements;

public sealed class AchievementLevel : ITenantOwned, IAuditable
{
    private AchievementLevel()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }
    public int Position { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int RequiredClasses { get; private set; }

    public static AchievementLevel Create(int position, LevelDefinition definition) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            Position = position,
            Name = definition.Name.Trim(),
            RequiredClasses = definition.RequiredClasses,
        };

    public LevelDefinition ToDefinition() => new(Name, RequiredClasses);
}
