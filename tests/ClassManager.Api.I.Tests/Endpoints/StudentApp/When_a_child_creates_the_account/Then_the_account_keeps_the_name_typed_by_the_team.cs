using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_child_creates_the_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_account_keeps_the_name_typed_by_the_team(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_account_keeps_the_name_typed_by_the_team_Run()
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
        (await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(childEmail))).EnsureSuccessStatusCode();

        await using var securityContext = fixture.CreateSecurityDbContext();
        (await securityContext.Users.SingleAsync(user => user.Email == childEmail)).FullName.ShouldBe(StudentAppRequests.ChildFullName);
    }
}
