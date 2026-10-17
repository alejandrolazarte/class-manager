namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_an_instructor_is_created_as_the_same_person_as_a_student;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_keep_one_user(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_keep_one_user_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var studentUserId = UserRows.UserIdOf(scenario.Tokens.AccessToken)!.Value;

        using var response = await scenario.Instructors.Business.HttpClient.PostInstructorAsync("Ana Gómez", scenario.Email, studentUserId);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var instructor = (await response.Content.ReadFromJsonAsync<Core.UseCases.Instructors.InstructorResponse>(ApiRequests.JsonOptions))!;
        (await fixture.UserIdOfInstructorAsync(scenario.Instructors.Business.Business.Id, instructor.Id)).ShouldBe(studentUserId);
    }
}
