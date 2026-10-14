using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_school_publishes_an_announcement_for_another_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_gets_no_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_gets_no_push_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();
        var owner = scenario.Instructors.Business.HttpClient;

        await owner.PublishAnnouncementAsync(new CreateAnnouncementCommand(
            "Solo para la otra clase", null, ClassGroupIds: [scenario.Instructors.OtherClassGroup.Id]));
        await owner.PublishAnnouncementAsync(new CreateAnnouncementCommand(
            "Para su clase", null, ClassGroupIds: [scenario.Instructors.InstructorClassGroup.Id]));

        (await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint)).Title.ShouldBe("Para su clase");
        fixture.ApiFactory.PushSender.SentTo(endpoint).ShouldNotContain(push => push.Title == "Solo para la otra clase");
    }
}
