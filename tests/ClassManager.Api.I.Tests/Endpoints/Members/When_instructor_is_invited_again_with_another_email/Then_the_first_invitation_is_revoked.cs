using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Members;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_instructor_is_invited_again_with_another_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_first_invitation_is_revoked(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_first_invitation_is_revoked_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        await business.HttpClient.InviteAsync(MemberRequests.UniqueInviteeEmail(), BusinessRole.Instructor, instructor.Id);
        var secondEmail = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(secondEmail, BusinessRole.Instructor, instructor.Id);

        var team = await business.HttpClient.GetFromJsonAsync<TeamResponse>(
            new Uri(ApiRoutes.Members, UriKind.Relative), ApiRequests.JsonOptions);

        team!.Invitations.Where(invitation => invitation.InstructorId == instructor.Id).Select(invitation => invitation.Email).ShouldBe([secondEmail]);
    }
}
