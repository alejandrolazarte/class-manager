using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_family_books_a_makeup;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_coach_of_that_class_is_notified(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_coach_of_that_class_is_notified_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var otherCoach = await fixture.SeedMemberAsync(
            scenario.Coaches.Business.Business.Id, BusinessRole.Coach, scenario.Coaches.OtherInstructorId);

        await scenario.NoticeAndBookOtherClassAsync();

        var notification = (await otherCoach.GetTeamNotificationsAsync())!.Items.Single();
        notification.Title.ShouldBe("Tomás viene a recuperar");
        notification.Body.ShouldStartWith(CoachScenario.OtherClassGroupName);
    }
}
