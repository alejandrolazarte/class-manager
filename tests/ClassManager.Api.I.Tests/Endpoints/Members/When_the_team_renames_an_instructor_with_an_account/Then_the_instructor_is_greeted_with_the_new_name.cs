using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_the_team_renames_an_instructor_with_an_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_instructor_is_greeted_with_the_new_name(ApiFixture fixture)
{
    private const string NewFullName = "Martina Ríos";

    [Fact]
    public async Task Then_the_instructor_is_greeted_with_the_new_name_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Instructor, instructor.Id);
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        await anonymousClient.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));
        var tokens = await anonymousClient.SignInAsync(email, MemberRequests.InviteePassword);
        using var instructorClient = fixture.CreateClientWithToken(tokens.AccessToken);
        using (var renamed = await business.HttpClient.PutAsJsonAsync(
            $"{ApiRoutes.Instructors}/{instructor.Id}", new UpdateInstructorRequest(NewFullName, email), ApiRequests.JsonOptions))
        {
            renamed.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        var member = await instructorClient.GetCurrentMemberAsync();

        member.FullName.ShouldBe(NewFullName);
    }
}
