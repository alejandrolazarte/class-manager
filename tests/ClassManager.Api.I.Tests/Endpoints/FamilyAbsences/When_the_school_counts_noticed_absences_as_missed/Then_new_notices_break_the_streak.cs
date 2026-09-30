using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyAbsences.When_the_school_counts_noticed_absences_as_missed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_new_notices_break_the_streak(ApiFixture fixture)
{
    [Fact]
    public async Task Then_new_notices_break_the_streak_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var business = scenario.Coaches.Business;
        (await business.HttpClient.PutAchievementSettingsAsync(AchievementRequests.SettingsWith(noticedAbsencesKeepStreak: false)))
            .EnsureSuccessStatusCode();

        (await scenario.Family.PutAbsenceAsync(
            scenario.Coaches.CoachStudentId, scenario.Coaches.CoachClassGroup.Id, CoachScenario.ClassDate)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);
        (await context.AbsenceNotices.SingleAsync(notice => notice.StudentId == scenario.Coaches.CoachStudentId)).KeepsStreak.ShouldBeFalse();
    }
}
