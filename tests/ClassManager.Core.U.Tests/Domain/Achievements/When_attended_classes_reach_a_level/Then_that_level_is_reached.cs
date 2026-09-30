using ClassManager.Core.Domain.Achievements;

namespace ClassManager.Core.U.Tests.Domain.Achievements.When_attended_classes_reach_a_level;

public sealed class Then_that_level_is_reached
{
    [Fact]
    public void Then_that_level_is_reached_Run()
    {
        var level = LevelLadder.LevelFor(LevelLadder.Defaults, 25);

        level.ShouldBe(3);
    }
}
