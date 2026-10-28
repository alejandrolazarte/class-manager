namespace ClassManager.Api.I.Tests.Endpoints.StudentAppFamily.When_the_instructor_comments_a_class_of_a_teenager;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_payer_gets_no_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_payer_gets_no_push_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        using var teenager = await fixture.SignInFamilyMemberAsync(owner, scenario.ClientId, "Lucía Pérez", StudentAppRequests.ChildBirthDate);
        var classGroupId = scenario.Instructors.InstructorClassGroup.Id;
        await owner.EnrollAsync(classGroupId, teenager.Student.Id, InstructorScenario.ClassDate);
        var endpoint = await scenario.SubscribeAsync();

        (await scenario.Instructors.Instructor.PutFeedbackAsync(
            classGroupId, InstructorScenario.ClassDate, teenager.Student.Id, "Comentario para Lucía")).EnsureSuccessStatusCode();
        (await scenario.Instructors.Instructor.PutFeedbackAsync(
            classGroupId, InstructorScenario.ClassDate, scenario.Instructors.InstructorStudentId, "Comentario para Tomás")).EnsureSuccessStatusCode();

        (await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint)).Body.ShouldBe("Comentario para Tomás");
        fixture.ApiFactory.PushSender.SentTo(endpoint).ShouldNotContain(push => push.Body == "Comentario para Lucía");
    }
}
