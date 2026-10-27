using ClassManager.Core.UseCases.Students;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_contact_who_attends_creates_the_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_account_gets_the_birth_date_typed_by_the_team(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_account_gets_the_birth_date_typed_by_the_team_Run()
    {
        var birthDateTypedByTheTeam = new DateOnly(1985, 6, 20);
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync(
            students: [new NewStudent(ApiRequests.ClientFullName, birthDateTypedByTheTeam, null)]);
        var contactEmail = StudentAppRequests.UniqueStudentEmail();
        (await business.HttpClient.PostStudentAppInvitationAsync(client.Id, contactEmail)).EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();

        (await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(contactEmail)))
            .EnsureSuccessStatusCode();

        await using var securityContext = fixture.CreateSecurityDbContext();
        (await securityContext.Users.SingleAsync(user => user.Email == contactEmail)).BirthDate.ShouldBe(birthDateTypedByTheTeam);
    }
}
