using ClassManager.Core.Domain.Users;
using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_an_instructor_is_created_with_the_email_of_a_student;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_asks_whether_it_is_the_same_person(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_asks_whether_it_is_the_same_person_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;

        using var response = await owner.PostAsJsonAsync(
            ApiRoutes.Instructors, new CreateInstructorCommand("Ana Gómez", scenario.Email), ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(UserErrorCodes.SamePerson);
        SamePersonRequests.SamePersonUserIdOf(problem).ShouldBe(UserRows.UserIdOf(scenario.Tokens.AccessToken));
    }
}
