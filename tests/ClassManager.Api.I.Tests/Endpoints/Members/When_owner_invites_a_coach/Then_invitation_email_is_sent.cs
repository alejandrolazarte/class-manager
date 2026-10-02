using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_owner_invites_a_coach;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_invitation_email_is_sent(ApiFixture fixture)
{
    [Fact]
    public async Task Then_invitation_email_is_sent_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var email = MemberRequests.UniqueInviteeEmail();

        await business.HttpClient.InviteAsync(email, BusinessRole.Coach, instructor.Id);

        fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email).ShouldNotBeNullOrEmpty();
    }
}
