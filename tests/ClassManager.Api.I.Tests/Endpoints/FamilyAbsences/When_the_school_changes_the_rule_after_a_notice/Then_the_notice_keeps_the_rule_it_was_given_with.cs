using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyAbsences.When_the_school_changes_the_rule_after_a_notice;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_notice_keeps_the_rule_it_was_given_with(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_notice_keeps_the_rule_it_was_given_with_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var business = scenario.Coaches.Business;
        (await scenario.Family.PutAbsenceAsync(
            scenario.Coaches.CoachStudentId, scenario.Coaches.CoachClassGroup.Id, CoachScenario.ClassDate)).EnsureSuccessStatusCode();

        (await business.HttpClient.PutBusinessAsync(SettingsRequests.SettingsOf(business, noticedAbsencesKeepStreak: false)))
            .EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);
        (await context.AbsenceNotices.SingleAsync(notice => notice.StudentId == scenario.Coaches.CoachStudentId)).KeepsStreak.ShouldBeTrue();
    }
}
