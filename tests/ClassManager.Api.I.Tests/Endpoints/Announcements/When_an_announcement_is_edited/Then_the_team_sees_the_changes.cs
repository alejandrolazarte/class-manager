using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_an_announcement_is_edited;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_the_changes(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_team_sees_the_changes_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var published = await scenario.Business.HttpClient.PublishAnnouncementAsync();

        await scenario.Business.HttpClient.UpdateAnnouncementAsync(published.Id, new UpdateAnnouncementRequest(
            "Muestra de fin de año", "Traé a tu familia.", AnnouncementCategory.Event, [scenario.InstructorClassGroup.Id], IsPinned: true));

        var announcement = (await scenario.Business.HttpClient.ListAnnouncementsAsync())!.Single();
        announcement.Title.ShouldBe("Muestra de fin de año");
        announcement.Body.ShouldBe("Traé a tu familia.");
        announcement.Category.ShouldBe(AnnouncementCategory.Event);
        announcement.IsPinned.ShouldBeTrue();
        announcement.PublishedAt.ShouldBe(published.PublishedAt);
        announcement.ClassGroups.ShouldBe([new AnnouncementClassGroupResponse(scenario.InstructorClassGroup.Id, InstructorScenario.InstructorClassGroupName)]);
    }
}
