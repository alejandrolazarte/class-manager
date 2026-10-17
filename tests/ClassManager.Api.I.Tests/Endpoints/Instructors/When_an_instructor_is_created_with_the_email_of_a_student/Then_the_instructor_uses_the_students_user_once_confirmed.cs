using ClassManager.Core.UseCases.Instructors;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_an_instructor_is_created_with_the_email_of_a_student;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_instructor_uses_the_students_user_once_confirmed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_instructor_uses_the_students_user_once_confirmed_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var businessId = scenario.Instructors.Business.Business.Id;
        var studentUserId = UserRows.UserIdOf(scenario.Tokens.AccessToken)!.Value;
        var usersBefore = await CountUsersAsync(businessId);

        using var response = await owner.PostAsJsonAsync(
            ApiRoutes.Instructors, new CreateInstructorCommand("Ana Gómez", scenario.Email, studentUserId), ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var instructor = (await response.Content.ReadFromJsonAsync<InstructorResponse>(ApiRequests.JsonOptions))!;
        (await fixture.UserIdOfInstructorAsync(businessId, instructor.Id)).ShouldBe(studentUserId);
        (await CountUsersAsync(businessId)).ShouldBe(usersBefore);
    }

    private async Task<int> CountUsersAsync(Guid businessId)
    {
        await using var context = fixture.CreateDbContext(businessId);
        return await context.Users.CountAsync();
    }
}
