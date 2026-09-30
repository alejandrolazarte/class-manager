using ClassManager.Core.Domain.Achievements;

namespace ClassManager.Core.U.Tests.Domain.Achievements.When_a_level_has_no_name;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var error = LevelLadder.Validate([new LevelDefinition("Inicial", 0), new LevelDefinition("  ", 10)]);

        error!.Code.ShouldBe(AchievementErrorCodes.LevelNameRequired);
    }
}
