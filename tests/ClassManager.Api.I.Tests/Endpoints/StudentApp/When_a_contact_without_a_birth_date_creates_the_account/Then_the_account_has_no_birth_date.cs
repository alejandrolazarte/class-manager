using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_contact_without_a_birth_date_creates_the_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_account_has_no_birth_date(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_account_has_no_birth_date_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        var contactEmail = StudentAppRequests.UniqueStudentEmail();
        (await business.HttpClient.PostStudentAppInvitationAsync(family.Id, contactEmail)).EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();

        (await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(contactEmail)))
            .EnsureSuccessStatusCode();

        await using var securityContext = fixture.CreateSecurityDbContext();
        (await securityContext.Users.SingleAsync(user => user.Email == contactEmail)).BirthDate.ShouldBeNull();
    }
}
