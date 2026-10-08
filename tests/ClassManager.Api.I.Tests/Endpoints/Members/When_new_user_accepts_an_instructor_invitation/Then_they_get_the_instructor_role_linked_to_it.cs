using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_new_user_accepts_an_instructor_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_get_the_instructor_role_linked_to_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_get_the_instructor_role_linked_to_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Instructor, instructor.Id);
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        var tokens = await anonymousClient.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));

        using var instructorClient = fixture.CreateClientWithToken(tokens.AccessToken);
        var member = await instructorClient.GetCurrentMemberAsync();
        member.BranchRole.ShouldBe(BusinessRole.Instructor);
        member.InstructorId.ShouldBe(instructor.Id);
        member.BusinessId.ShouldBe(business.Business.Id);
    }
}
