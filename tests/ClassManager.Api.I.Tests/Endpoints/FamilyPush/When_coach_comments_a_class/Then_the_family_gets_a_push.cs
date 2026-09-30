using ClassManager.Infrastructure.WebPush;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyPush.When_coach_comments_a_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_family_gets_a_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_family_gets_a_push_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();

        (await scenario.Coaches.Coach.PutFeedbackAsync(
            scenario.Coaches.CoachClassGroup.Id, CoachScenario.ClassDate, scenario.Coaches.CoachStudentId, "Muy buena patada")).EnsureSuccessStatusCode();

        var push = await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint);
        push.ShouldBe(new PushMessage($"{CoachScenario.CoachFullName} comentó la clase de Tomás", "Muy buena patada", "/family/news"));
    }
}
