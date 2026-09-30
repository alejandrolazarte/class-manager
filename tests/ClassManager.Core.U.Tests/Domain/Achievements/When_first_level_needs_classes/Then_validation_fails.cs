using ClassManager.Core.Domain.Achievements;

namespace ClassManager.Core.U.Tests.Domain.Achievements.When_first_level_needs_classes;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var error = LevelLadder.Validate([new LevelDefinition("Inicial", 5), new LevelDefinition("Base", 10)]);

        error!.Code.ShouldBe(AchievementErrorCodes.FirstLevelStartsAtZero);
    }
}
