using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_child_is_invited_to_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_child_signs_in_and_sees_the_family(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_child_signs_in_and_sees_the_family_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        var childEmail = StudentAppRequests.UniqueStudentEmail();
        using (var invitation = await business.HttpClient.PostChildAppInvitationAsync(
            family.Id, family.Students.Single().Id, childEmail, StudentAppRequests.ChildBirthDate))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var accepted = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(childEmail));
        var tokens = (await accepted.Content.ReadFromJsonAsync<TokenResponse>(ApiRequests.JsonOptions))!;
        using var child = fixture.CreateClientWithToken(tokens.AccessToken);

        var home = await child.GetStudentAppHomeAsync();

        home!.Students.Single().FullName.ShouldBe(StudentAppRequests.ChildFullName);
    }
}
