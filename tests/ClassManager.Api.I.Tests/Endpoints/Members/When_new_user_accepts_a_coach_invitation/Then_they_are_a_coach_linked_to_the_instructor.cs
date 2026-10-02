using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_new_user_accepts_a_coach_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_are_a_coach_linked_to_the_instructor(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_are_a_coach_linked_to_the_instructor_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Coach, instructor.Id);
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        var tokens = await anonymousClient.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));

        using var coachClient = fixture.CreateClientWithToken(tokens.AccessToken);
        var member = await coachClient.GetCurrentMemberAsync();
        member.BranchRole.ShouldBe(BusinessRole.Coach);
        member.InstructorId.ShouldBe(instructor.Id);
        member.BusinessId.ShouldBe(business.Business.Id);
    }
}
