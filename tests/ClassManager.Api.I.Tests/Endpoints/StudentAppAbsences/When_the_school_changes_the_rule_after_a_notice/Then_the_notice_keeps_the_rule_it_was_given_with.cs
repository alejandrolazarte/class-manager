using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppAbsences.When_the_school_changes_the_rule_after_a_notice;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_notice_keeps_the_rule_it_was_given_with(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_notice_keeps_the_rule_it_was_given_with_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var business = scenario.Instructors.Business;
        (await scenario.Student.PutAbsenceAsync(
            scenario.Instructors.InstructorStudentId, scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate)).EnsureSuccessStatusCode();

        (await business.HttpClient.PutAchievementSettingsAsync(AchievementRequests.SettingsWith(noticedAbsencesKeepStreak: false)))
            .EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);
        (await context.AbsenceNotices.SingleAsync(notice => notice.StudentId == scenario.Instructors.InstructorStudentId)).KeepsStreak.ShouldBeTrue();
    }
}
