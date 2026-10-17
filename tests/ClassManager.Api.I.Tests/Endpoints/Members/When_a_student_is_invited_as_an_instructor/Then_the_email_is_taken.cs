using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_a_student_is_invited_as_an_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_email_is_taken(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_email_is_taken_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var instructor = await owner.CreateInstructorAsync("Ana Gómez");

        using var response = await owner.PostInvitationAsync(scenario.Email, BusinessRole.Instructor, instructor.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(UserErrorCodes.EmailTaken);
    }
}
