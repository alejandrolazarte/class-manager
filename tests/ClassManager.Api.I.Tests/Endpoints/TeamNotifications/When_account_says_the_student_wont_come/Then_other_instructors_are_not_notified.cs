using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_account_says_the_student_wont_come;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_other_instructors_are_not_notified(ApiFixture fixture)
{
    [Fact]
    public async Task Then_other_instructors_are_not_notified_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var otherInstructor = await fixture.SeedMemberAsync(
            scenario.Instructors.Business.Business.Id, BusinessRole.Instructor, scenario.Instructors.OtherInstructorId);

        (await scenario.Student.PutAbsenceAsync(
            scenario.Instructors.InstructorStudentId, scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate.AddDays(7))).EnsureSuccessStatusCode();

        (await otherInstructor.GetTeamNotificationsAsync())!.Items.ShouldBeEmpty();
    }
}
