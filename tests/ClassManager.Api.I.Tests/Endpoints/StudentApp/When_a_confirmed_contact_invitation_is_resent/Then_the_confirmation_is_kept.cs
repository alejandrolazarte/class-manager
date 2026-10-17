using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_confirmed_contact_invitation_is_resent;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_confirmation_is_kept(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_confirmation_is_kept_Run()
    {
        var instructors = await fixture.SeedInstructorScenarioAsync();
        var fees = await instructors.Business.HttpClient.GetMonthlyFeesAsync();
        var clientId = fees!.Clients.Single(client => client.StudentNames.Contains(InstructorScenario.InstructorStudentFullName)).ClientId;
        var email = StudentAppRequests.UniqueStudentEmail();
        using var firstInvitation = await instructors.Business.HttpClient.PostStudentAppInvitationAsync(clientId, email);

        using var resentInvitation = await instructors.Business.HttpClient.PostStudentAppInvitationAsync(clientId, email, confirmsContactIsAdult: false);

        await using var context = fixture.CreateDbContext(instructors.Business.Business.Id);
        var invitations = await context.ClientInvitations.AsNoTracking().Where(invitation => invitation.ClientId == clientId).ToListAsync();
        var ownerUserId = await fixture.FindUserIdByAccountAsync(instructors.Business.Business.Id, instructors.Business.OwnerAccountId);
        resentInvitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        invitations.Single(invitation => invitation.RevokedAt == null).ContactAdultConfirmedByUserId.ShouldBe(ownerUserId);
    }
}
