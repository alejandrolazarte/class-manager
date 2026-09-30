using ClassManager.Core.Domain.Achievements;

namespace ClassManager.Api.I.Tests.Endpoints.Achievements.When_owner_opens_achievement_settings;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_default_levels_and_rule_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_default_levels_and_rule_are_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        var settings = await business.HttpClient.GetAchievementSettingsAsync();

        settings!.NoticedAbsencesKeepStreak.ShouldBeTrue();
        settings.Levels.ShouldBe(LevelLadder.Defaults);
    }
}
