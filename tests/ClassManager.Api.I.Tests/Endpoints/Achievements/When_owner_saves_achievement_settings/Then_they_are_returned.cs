namespace ClassManager.Api.I.Tests.Endpoints.Achievements.When_owner_saves_achievement_settings;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_are_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PutAchievementSettingsAsync(
            AchievementRequests.SettingsWith(noticedAbsencesKeepStreak: false, levels: AchievementRequests.ThreeLevels));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var settings = await business.HttpClient.GetAchievementSettingsAsync();
        settings!.NoticedAbsencesKeepStreak.ShouldBeFalse();
        settings.Levels.ShouldBe(AchievementRequests.ThreeLevels);
    }
}
