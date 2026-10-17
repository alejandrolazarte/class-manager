using ClassManager.Core.UseCases.Students;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Students.When_a_student_is_added_as_the_same_person_as_an_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_keep_one_user(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_keep_one_user_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var email = MemberRequests.UniqueInviteeEmail();
        var instructor = await owner.CreateInstructorAsync("Ana Gómez", email);
        var instructorUserId = await fixture.UserIdOfInstructorAsync(business.Business.Id, instructor.Id);
        var client = await owner.RegisterClientAsync();

        using var response = await owner.PostStudentAsync(client.Id, "Ana Gómez", email, instructorUserId);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var student = (await response.Content.ReadFromJsonAsync<StudentResponse>(ApiRequests.JsonOptions))!;
        await using var context = fixture.CreateDbContext(business.Business.Id);
        (await context.Students.Where(row => row.Id == student.Id).Select(row => row.UserId).SingleAsync()).ShouldBe(instructorUserId);
    }
}
