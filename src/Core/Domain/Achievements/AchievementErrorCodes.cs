namespace ClassManager.Core.Domain.Achievements;

public static class AchievementErrorCodes
{
    public const string LevelCount = "achievements.level_count";
    public const string LevelNameRequired = "achievements.level_name_required";
    public const string LevelNameTooLong = "achievements.level_name_too_long";
    public const string FirstLevelStartsAtZero = "achievements.first_level_starts_at_zero";
    public const string ClassesMustIncrease = "achievements.classes_must_increase";
}
