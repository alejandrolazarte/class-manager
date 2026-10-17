using ClassManager.Core.Domain.Users;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_an_instructor_is_created_with_the_email_of_a_student;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_asks_whether_it_is_the_same_person(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_asks_whether_it_is_the_same_person_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        using var response = await scenario.Instructors.Business.HttpClient.PostInstructorAsync("Ana Gómez", scenario.Email);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(UserErrorCodes.SamePerson);
        SamePersonRequests.SamePersonUserIdOf(problem).ShouldBe(UserRows.UserIdOf(scenario.Tokens.AccessToken));
    }
}
