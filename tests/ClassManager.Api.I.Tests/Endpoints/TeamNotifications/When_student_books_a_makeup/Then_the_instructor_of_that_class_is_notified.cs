using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_student_books_a_makeup;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_instructor_of_that_class_is_notified(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_instructor_of_that_class_is_notified_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var otherInstructor = await fixture.SeedMemberAsync(
            scenario.Instructors.Business.Business.Id, BusinessRole.Instructor, scenario.Instructors.OtherInstructorId);

        await scenario.NoticeAndBookOtherClassAsync();

        var notification = (await otherInstructor.GetTeamNotificationsAsync())!.Items.Single();
        notification.Title.ShouldBe("Tomás viene a recuperar");
        notification.Body.ShouldStartWith(InstructorScenario.OtherClassGroupName);
    }
}
