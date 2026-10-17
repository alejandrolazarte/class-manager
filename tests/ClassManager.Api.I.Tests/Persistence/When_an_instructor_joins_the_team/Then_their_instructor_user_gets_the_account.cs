using ClassManager.Core.Domain.Businesses;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_an_instructor_joins_the_team;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_their_instructor_user_gets_the_account(ApiFixture fixture)
{
    [Fact]
    public async Task Then_their_instructor_user_gets_the_account_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructorProfile = await business.HttpClient.CreateInstructorAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Instructor, instructorProfile.Id);
        using var anonymous = fixture.ApiFactory.CreateClient();

        await anonymous.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));

        await using var context = fixture.CreateDbContext(business.Business.Id);
        var member = await context.BusinessMembers.AsNoTracking().SingleAsync(row => row.InstructorId == instructorProfile.Id);
        var instructor = await context.Instructors.AsNoTracking().SingleAsync(row => row.Id == instructorProfile.Id);
        var user = await context.Users.AsNoTracking().SingleAsync(row => row.Id == member.UserId);
        member.UserId.ShouldBe(instructor.UserId);
        user.AccountId.ShouldBe(member.AccountId);
    }
}
