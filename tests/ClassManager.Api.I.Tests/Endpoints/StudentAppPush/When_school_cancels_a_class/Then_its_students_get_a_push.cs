namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_school_cancels_a_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_students_get_a_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_students_get_a_push_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();

        (await scenario.Instructors.Business.HttpClient.PutSessionCancellationAsync(
            scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate, "Feriado")).EnsureSuccessStatusCode();

        var push = await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint);
        push.Title.ShouldBe($"Se suspende {InstructorScenario.InstructorClassGroupName}");
        push.Body.ShouldBe("El jueves 24/9 a las 18:00 no hay clase. Feriado");
    }
}
