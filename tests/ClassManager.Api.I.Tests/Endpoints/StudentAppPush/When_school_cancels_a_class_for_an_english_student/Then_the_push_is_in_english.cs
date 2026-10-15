using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_school_cancels_a_class_for_an_english_student;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_push_is_in_english(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_push_is_in_english_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        (await scenario.Student.PutMyLanguageAsync(AccountLanguage.English)).EnsureSuccessStatusCode();
        var endpoint = await scenario.SubscribeAsync();

        (await scenario.Instructors.Business.HttpClient.PutSessionCancellationAsync(
            scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate, "Feriado")).EnsureSuccessStatusCode();

        var push = await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint);
        push.Title.ShouldBe($"{InstructorScenario.InstructorClassGroupName} is cancelled");
        push.Body.ShouldBe("There's no class on Thursday, Sep 24 at 18:00. Feriado");
    }
}
