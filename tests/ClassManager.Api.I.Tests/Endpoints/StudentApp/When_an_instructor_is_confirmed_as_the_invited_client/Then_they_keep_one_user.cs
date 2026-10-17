using ClassManager.Core.Domain.Businesses;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_an_instructor_is_confirmed_as_the_invited_client;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_keep_one_user(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_keep_one_user_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var businessId = business.Business.Id;
        var email = MemberRequests.UniqueInviteeEmail();
        var instructor = await owner.CreateInstructorAsync("Ana Gómez");
        await owner.InviteAsync(email, BusinessRole.Instructor, instructor.Id);
        using var anonymous = fixture.ApiFactory.CreateClient();
        await anonymous.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));
        var instructorUserId = await fixture.UserIdOfInstructorAsync(businessId, instructor.Id);
        var studentId = await owner.RegisterStudentAsync("Ana Gómez", "11 4444-5555");
        var clientId = await fixture.ClientIdOfStudentAsync(businessId, studentId);
        using (var invitation = await owner.PostStudentAppInvitationAsync(clientId, email, samePersonUserId: instructorUserId))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var accepted = await anonymous.PostAcceptStudentAppInvitationAsync(
            fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(email), MemberRequests.InviteePassword);

        accepted.StatusCode.ShouldBe(HttpStatusCode.OK);
        await using var context = fixture.CreateDbContext(businessId);
        var client = await context.Clients.AsNoTracking().Include(row => row.User).SingleAsync(row => row.Id == clientId);
        client.UserId.ShouldBe(instructorUserId);
        client.User.PhoneNumber.ShouldNotBeNull();
        (await context.ClientAccounts.Where(account => account.ClientId == clientId).Select(account => account.UserId).SingleAsync()).ShouldBe(instructorUserId);
    }
}
