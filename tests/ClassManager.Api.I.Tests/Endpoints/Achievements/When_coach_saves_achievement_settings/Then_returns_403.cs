namespace ClassManager.Api.I.Tests.Endpoints.Achievements.When_coach_saves_achievement_settings;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        using var response = await scenario.Coach.PutAchievementSettingsAsync(AchievementRequests.SettingsWith());

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
