using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_an_announcement_is_for_some_classes;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_its_classes(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_team_sees_its_classes_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var visibleUntil = InstructorScenario.ClassDate.AddDays(7);
        await scenario.Business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand(
            "Muestra de fin de año", null, AnnouncementCategory.Event, [scenario.InstructorClassGroup.Id], VisibleUntil: visibleUntil));

        var announcement = (await scenario.Business.HttpClient.ListAnnouncementsAsync())!.Single();

        announcement.Category.ShouldBe(AnnouncementCategory.Event);
        announcement.VisibleUntil.ShouldBe(visibleUntil);
        announcement.ClassGroups.ShouldBe([new AnnouncementClassGroupResponse(scenario.InstructorClassGroup.Id, InstructorScenario.InstructorClassGroupName)]);
    }
}
