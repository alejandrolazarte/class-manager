using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_instructor_opens_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_name_set_by_the_team_is_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_name_set_by_the_team_is_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Instructor, instructor.Id);
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        await anonymousClient.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));
        var tokens = await anonymousClient.SignInAsync(email, MemberRequests.InviteePassword);
        using var instructorClient = fixture.CreateClientWithToken(tokens.AccessToken);

        var member = await instructorClient.GetCurrentMemberAsync();

        member.FullName.ShouldBe(instructor.FullName);
    }
}
