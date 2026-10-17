using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_a_student_is_invited_as_an_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_asks_whether_it_is_the_same_person(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_asks_whether_it_is_the_same_person_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var instructor = await owner.CreateInstructorAsync("Ana Gómez");

        using var response = await owner.PostInvitationAsync(scenario.Email, BusinessRole.Instructor, instructor.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(UserErrorCodes.SamePerson);
        SamePersonRequests.SamePersonUserIdOf(problem).ShouldBe(UserRows.UserIdOf(scenario.Tokens.AccessToken));
    }
}
