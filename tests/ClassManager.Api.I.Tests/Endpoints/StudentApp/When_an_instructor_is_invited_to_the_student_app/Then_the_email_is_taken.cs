using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_an_instructor_is_invited_to_the_student_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_email_is_taken(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_email_is_taken_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var email = MemberRequests.UniqueInviteeEmail();
        var instructor = await owner.CreateInstructorAsync("Ana Gómez");
        await owner.InviteAsync(email, BusinessRole.Instructor, instructor.Id);
        using var anonymous = fixture.ApiFactory.CreateClient();
        await anonymous.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));
        var studentId = await owner.RegisterStudentAsync("Ana Gómez", "11 4444-5555");
        await using var context = fixture.CreateDbContext(business.Business.Id);
        var clientId = await context.Students.Where(student => student.Id == studentId).Select(student => student.ClientId).SingleAsync();

        using var response = await owner.PostStudentAppInvitationAsync(clientId, email);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(UserErrorCodes.EmailTaken);
    }
}
