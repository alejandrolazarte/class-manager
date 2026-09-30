using ClassManager.Core.Domain.Achievements;

namespace ClassManager.Api.I.Tests.Endpoints.Achievements.When_owner_saves_levels_that_do_not_increase;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PutAchievementSettingsAsync(
            AchievementRequests.SettingsWith(levels: [new LevelDefinition("Inicial", 0), new LevelDefinition("Base", 0)]));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(AchievementErrorCodes.ClassesMustIncrease);
    }
}
