using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_an_instructor_is_invited_to_the_student_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_asks_whether_it_is_the_same_person(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_asks_whether_it_is_the_same_person_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var email = MemberRequests.UniqueInviteeEmail();
        var instructor = await owner.CreateInstructorAsync("Ana Gómez");
        await owner.InviteAsync(email, BusinessRole.Instructor, instructor.Id);
        var studentId = await owner.RegisterStudentAsync("Ana Gómez", "11 4444-5555");
        var clientId = await fixture.ClientIdOfStudentAsync(business.Business.Id, studentId);

        using var response = await owner.PostStudentAppInvitationAsync(clientId, email);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(UserErrorCodes.SamePerson);
        SamePersonRequests.SamePersonUserIdOf(problem).ShouldBe(await fixture.UserIdOfInstructorAsync(business.Business.Id, instructor.Id));
    }
}
