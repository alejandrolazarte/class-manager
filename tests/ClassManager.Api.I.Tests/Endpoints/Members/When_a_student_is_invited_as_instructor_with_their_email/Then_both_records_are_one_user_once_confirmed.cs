using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Members;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_a_student_is_invited_as_instructor_with_their_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_both_records_are_one_user_once_confirmed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_both_records_are_one_user_once_confirmed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(email);
        var instructor = await business.HttpClient.CreateInstructorAsync(ApiRequests.ClientFullName);
        await using var context = fixture.CreateDbContext(business.Business.Id);
        var contactUserId = await context.Clients.Where(client => client.Id == family.Id).Select(client => client.UserId).SingleAsync();
        var instructorUserId = await context.Instructors.Where(row => row.Id == instructor.Id).Select(row => row.UserId).SingleAsync();

        using var response = await business.HttpClient.PostAsJsonAsync(
            MemberRequests.InvitationsRoute,
            new InviteMemberCommand(email, BusinessRole.Instructor, instructor.Id, SamePersonUserId: contactUserId),
            ApiRequests.JsonOptions);

        var users = new
        {
            Instructor = await context.Instructors.Where(row => row.Id == instructor.Id).Select(row => row.UserId).SingleAsync(),
            LeftOverIsGone = !await context.Users.AnyAsync(user => user.Id == instructorUserId),
        };
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        users.ShouldBe(new { Instructor = contactUserId, LeftOverIsGone = true });
    }
}
