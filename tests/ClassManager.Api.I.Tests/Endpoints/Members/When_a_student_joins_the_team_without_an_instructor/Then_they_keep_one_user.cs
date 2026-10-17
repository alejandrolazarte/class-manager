using ClassManager.Core.Domain.Businesses;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_a_student_joins_the_team_without_an_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_keep_one_user(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_keep_one_user_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var accountId = UserRows.AccountIdOf(scenario.Tokens.AccessToken);
        await scenario.Instructors.Business.HttpClient.InviteAsync(scenario.Email, BusinessRole.Viewer);
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var accepted = await anonymous.PostAcceptInvitationAsync(
            fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(scenario.Email), StudentAppRequests.StudentAppPassword);

        accepted.StatusCode.ShouldBe(HttpStatusCode.OK);
        await using var context = fixture.CreateDbContext(scenario.Instructors.Business.Business.Id);
        var userIds = await context.Users.Where(user => user.AccountId == accountId).Select(user => user.Id).ToListAsync();
        var memberUserId = await context.BusinessMembers.Where(member => member.User.AccountId == accountId).Select(member => member.UserId).SingleAsync();
        userIds.ShouldHaveSingleItem().ShouldBe(memberUserId);
    }
}
