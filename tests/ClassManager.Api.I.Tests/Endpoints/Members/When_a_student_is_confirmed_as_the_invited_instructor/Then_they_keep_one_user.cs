using ClassManager.Core.Domain.Businesses;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_a_student_is_confirmed_as_the_invited_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_keep_one_user(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_keep_one_user_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var businessId = scenario.Instructors.Business.Business.Id;
        var studentUserId = UserRows.UserIdOf(scenario.Tokens.AccessToken)!.Value;
        var instructor = await owner.CreateInstructorAsync("Ana Gómez");
        var previousInstructorUserId = await fixture.UserIdOfInstructorAsync(businessId, instructor.Id);
        using (var invitation = await owner.PostInvitationAsync(scenario.Email, BusinessRole.Instructor, instructor.Id, studentUserId))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var accepted = await anonymous.PostAcceptInvitationAsync(
            fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(scenario.Email), StudentAppRequests.StudentAppPassword);

        accepted.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await fixture.UserIdOfInstructorAsync(businessId, instructor.Id)).ShouldBe(studentUserId);
        await using var context = fixture.CreateDbContext(businessId);
        var accountId = UserRows.AccountIdOf(scenario.Tokens.AccessToken);
        (await context.Users.CountAsync(user => user.AccountId == accountId)).ShouldBe(1);
        (await context.Users.AnyAsync(user => user.Id == previousInstructorUserId)).ShouldBeFalse();
        (await context.BusinessMembers.Where(member => member.InstructorId == instructor.Id).Select(member => member.UserId).SingleAsync()).ShouldBe(studentUserId);
    }
}
