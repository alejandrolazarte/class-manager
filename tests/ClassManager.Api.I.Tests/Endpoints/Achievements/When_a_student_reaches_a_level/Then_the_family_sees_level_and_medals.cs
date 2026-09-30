using ClassManager.Core.Domain.Achievements;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Achievements.When_a_student_reaches_a_level;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_family_sees_level_and_medals(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_family_sees_level_and_medals_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        (await owner.PutAchievementSettingsAsync(AchievementRequests.SettingsWith(levels: AchievementRequests.ThreeLevels))).EnsureSuccessStatusCode();
        (await owner.PutAttendanceAsync(
            scenario.Coaches.CoachClassGroup.Id, CoachScenario.ClassDate, scenario.Coaches.CoachStudentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        var home = await scenario.Family.GetFamilyHomeAsync();

        home!.Levels.ShouldBe(AchievementRequests.ThreeLevels);
        var attendance = home.Students.Single().Attendance;
        attendance.Level.ShouldBe(2);
        attendance.Medals.ShouldBe([Medal.FirstClass, Medal.LeveledUp]);
    }
}
