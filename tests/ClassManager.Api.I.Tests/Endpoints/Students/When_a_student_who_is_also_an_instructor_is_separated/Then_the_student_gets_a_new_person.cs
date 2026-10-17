using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Students.When_a_student_who_is_also_an_instructor_is_separated;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_gets_a_new_person(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_gets_a_new_person_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var email = MemberRequests.UniqueInviteeEmail();
        var instructor = await owner.CreateInstructorAsync("Ana Gómez", email);
        var instructorUserId = await fixture.UserIdOfInstructorAsync(business.Business.Id, instructor.Id);
        var client = await owner.RegisterClientAsync();
        using var added = await owner.PostStudentAsync(client.Id, "Ana Gómez", email, instructorUserId);
        var student = (await added.Content.ReadFromJsonAsync<Core.UseCases.Students.StudentResponse>(ApiRequests.JsonOptions))!;

        using var response = await owner.PostSeparationAsync($"{ApiRoutes.Students}/{student.Id}", "Martina Gómez");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        await using var context = fixture.CreateDbContext(business.Business.Id);
        var studentUserId = await context.Students.Where(row => row.Id == student.Id).Select(row => row.UserId).SingleAsync();
        studentUserId.ShouldNotBe(instructorUserId);
        (await fixture.UserIdOfInstructorAsync(business.Business.Id, instructor.Id)).ShouldBe(instructorUserId);
    }
}
