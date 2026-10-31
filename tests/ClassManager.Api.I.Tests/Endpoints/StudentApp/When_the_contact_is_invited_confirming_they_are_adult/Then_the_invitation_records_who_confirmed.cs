using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_the_contact_is_invited_confirming_they_are_adult;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_invitation_records_who_confirmed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_invitation_records_who_confirmed_Run()
    {
        var instructors = await fixture.SeedInstructorScenarioAsync();
        var fees = await instructors.Business.HttpClient.GetMonthlyFeesAsync();
        var clientId = fees!.ClientIdOf(InstructorScenario.InstructorStudentFullName);
        using var response = await instructors.Business.HttpClient.PostStudentAppInvitationAsync(clientId, StudentAppRequests.UniqueStudentEmail());

        await using var context = fixture.CreateDbContext(instructors.Business.Business.Id);
        var invitation = await context.ClientInvitations.AsNoTracking().SingleAsync(clientInvitation => clientInvitation.ClientId == clientId);
        var ownerUserId = await fixture.FindUserIdByAccountAsync(instructors.Business.Business.Id, instructors.Business.OwnerAccountId);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        invitation.ContactAdultConfirmedByUserId.ShouldBe(ownerUserId);
    }
}
