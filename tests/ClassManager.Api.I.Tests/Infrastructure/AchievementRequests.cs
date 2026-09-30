using ClassManager.Core.Domain.Achievements;
using ClassManager.Core.UseCases.Achievements;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class AchievementRequests
{
    public static readonly IReadOnlyList<LevelDefinition> ThreeLevels =
    [
        new("Pececito", 0),
        new("Delfín", 1),
        new("Tiburón", 20),
    ];

    private static string SettingsPath => ApiRoutes.Business + ApiRoutes.Achievements;

    public static UpdateAchievementSettingsCommand SettingsWith(bool noticedAbsencesKeepStreak = true, IReadOnlyList<LevelDefinition>? levels = null) =>
        new(noticedAbsencesKeepStreak, levels ?? LevelLadder.Defaults);

    public static Task<AchievementSettingsResponse?> GetAchievementSettingsAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<AchievementSettingsResponse>(new Uri(SettingsPath, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutAchievementSettingsAsync(this HttpClient httpClient, UpdateAchievementSettingsCommand command) =>
        httpClient.PutAsJsonAsync(SettingsPath, command, ApiRequests.JsonOptions);
}
