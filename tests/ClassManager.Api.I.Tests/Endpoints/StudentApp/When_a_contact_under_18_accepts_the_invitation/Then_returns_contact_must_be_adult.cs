using ClassManager.Core.Domain.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_contact_under_18_accepts_the_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_contact_must_be_adult(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_contact_must_be_adult_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        var contactEmail = StudentAppRequests.UniqueStudentEmail();
        (await business.HttpClient.PostStudentAppInvitationAsync(family.Id, contactEmail)).EnsureSuccessStatusCode();
        var fifteenYearsAgo = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime).AddYears(-15);
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostAcceptStudentAppInvitationAsync(
            fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(contactEmail),
            fifteenYearsAgo);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(ClientErrorCodes.ContactMustBeAdult);
    }
}
