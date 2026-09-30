using ClassManager.Core.Domain.Achievements;

namespace ClassManager.Core.U.Tests.Domain.Achievements.When_attended_classes_are_below_the_second_level;

public sealed class Then_the_first_level_is_kept
{
    [Fact]
    public void Then_the_first_level_is_kept_Run()
    {
        var level = LevelLadder.LevelFor(LevelLadder.Defaults, 9);

        level.ShouldBe(1);
    }
}
