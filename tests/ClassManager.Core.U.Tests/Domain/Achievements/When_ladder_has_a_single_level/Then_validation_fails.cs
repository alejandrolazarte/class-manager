using ClassManager.Core.Domain.Achievements;

namespace ClassManager.Core.U.Tests.Domain.Achievements.When_ladder_has_a_single_level;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var error = LevelLadder.Validate([new LevelDefinition("Inicial", 0)]);

        error!.Code.ShouldBe(AchievementErrorCodes.LevelCount);
    }
}
