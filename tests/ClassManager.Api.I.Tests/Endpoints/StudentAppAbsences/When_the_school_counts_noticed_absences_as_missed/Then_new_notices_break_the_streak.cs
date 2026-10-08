using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppAbsences.When_the_school_counts_noticed_absences_as_missed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_new_notices_break_the_streak(ApiFixture fixture)
{
    [Fact]
    public async Task Then_new_notices_break_the_streak_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var business = scenario.Instructors.Business;
        (await business.HttpClient.PutAchievementSettingsAsync(AchievementRequests.SettingsWith(noticedAbsencesKeepStreak: false)))
            .EnsureSuccessStatusCode();

        (await scenario.Student.PutAbsenceAsync(
            scenario.Instructors.InstructorStudentId, scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);
        (await context.AbsenceNotices.SingleAsync(notice => notice.StudentId == scenario.Instructors.InstructorStudentId)).KeepsStreak.ShouldBeFalse();
    }
}
