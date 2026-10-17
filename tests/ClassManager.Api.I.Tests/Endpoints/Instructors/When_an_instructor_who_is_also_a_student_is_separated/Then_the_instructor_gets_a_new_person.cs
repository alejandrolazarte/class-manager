using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_an_instructor_who_is_also_a_student_is_separated;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_instructor_gets_a_new_person(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_instructor_gets_a_new_person_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var businessId = scenario.Instructors.Business.Business.Id;
        var (instructorId, studentUserId) = await fixture.SeedInstructorWhoIsAStudentAsync(scenario);

        using var response = await scenario.Instructors.Business.HttpClient.PostSeparationAsync(
            $"{ApiRoutes.Instructors}/{instructorId}", "Martina Gómez");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var instructorUserId = await fixture.UserIdOfInstructorAsync(businessId, instructorId);
        instructorUserId.ShouldNotBe(studentUserId);
        await using var context = fixture.CreateDbContext(businessId);
        (await context.Users.Where(user => user.Id == instructorUserId).Select(user => user.FullName).SingleAsync()).ShouldBe("Martina Gómez");
        (await context.Clients.AnyAsync(client => client.Id == scenario.ClientId && client.UserId == studentUserId)).ShouldBeTrue();
    }
}
