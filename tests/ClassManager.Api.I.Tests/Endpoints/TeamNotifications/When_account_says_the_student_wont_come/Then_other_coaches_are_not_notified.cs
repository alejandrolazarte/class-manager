using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_account_says_the_student_wont_come;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_other_coaches_are_not_notified(ApiFixture fixture)
{
    [Fact]
    public async Task Then_other_coaches_are_not_notified_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var otherCoach = await fixture.SeedMemberAsync(
            scenario.Coaches.Business.Business.Id, BusinessRole.Coach, scenario.Coaches.OtherInstructorId);

        (await scenario.Student.PutAbsenceAsync(
            scenario.Coaches.CoachStudentId, scenario.Coaches.CoachClassGroup.Id, CoachScenario.ClassDate.AddDays(7))).EnsureSuccessStatusCode();

        (await otherCoach.GetTeamNotificationsAsync())!.Items.ShouldBeEmpty();
    }
}
