using ClassManager.Core.Common;

namespace ClassManager.Core.Domain.Achievements;

public static class LevelLadder
{
    public const int MinLevels = 2;
    public const int MaxLevels = 10;
    public const int NameMaxLength = 40;
    public const string LevelsFieldName = "Levels";

    private const string LevelCountMessage = "Add between 2 and 10 levels.";
    private const string NameRequiredMessage = "Every level needs a name.";
    private const string NameTooLongMessage = "Level names must be at most 40 characters.";
    private const string FirstLevelMessage = "The first level must start at 0 classes.";
    private const string IncreaseMessage = "Each level must need more classes than the previous one.";

    public static readonly IReadOnlyList<LevelDefinition> Defaults =
    [
        new("Inicial", 0),
        new("Base", 10),
        new("Explorador", 25),
        new("Intermedio", 50),
        new("Avanzado", 100),
        new("Experto", 200),
    ];

    public static ResultError? Validate(IReadOnlyList<LevelDefinition> levels)
    {
        if (levels.Count is < MinLevels or > MaxLevels)
        {
            return Error(AchievementErrorCodes.LevelCount, LevelCountMessage);
        }

        if (levels.Any(level => string.IsNullOrWhiteSpace(level.Name)))
        {
            return Error(AchievementErrorCodes.LevelNameRequired, NameRequiredMessage);
        }

        if (levels.Any(level => level.Name.Trim().Length > NameMaxLength))
        {
            return Error(AchievementErrorCodes.LevelNameTooLong, NameTooLongMessage);
        }

        if (levels[0].RequiredClasses != 0)
        {
            return Error(AchievementErrorCodes.FirstLevelStartsAtZero, FirstLevelMessage);
        }

        return levels.Zip(levels.Skip(1)).Any(pair => pair.Second.RequiredClasses <= pair.First.RequiredClasses)
            ? Error(AchievementErrorCodes.ClassesMustIncrease, IncreaseMessage)
            : null;
    }

    public static int LevelFor(IReadOnlyList<LevelDefinition> levels, int attendedClasses) =>
        Math.Max(levels.Count(level => level.RequiredClasses <= attendedClasses), 1);

    private static ResultError Error(string code, string message) =>
        new(code, message, ErrorKind.Validation) { FieldName = LevelsFieldName };
}
