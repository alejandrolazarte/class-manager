using ClassManager.Core.Domain.Achievements;

namespace ClassManager.Core.U.Tests.Domain.Achievements.When_level_classes_do_not_increase;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var error = LevelLadder.Validate([new LevelDefinition("Inicial", 0), new LevelDefinition("Base", 10), new LevelDefinition("Explorador", 10)]);

        error!.Code.ShouldBe(AchievementErrorCodes.ClassesMustIncrease);
    }
}
