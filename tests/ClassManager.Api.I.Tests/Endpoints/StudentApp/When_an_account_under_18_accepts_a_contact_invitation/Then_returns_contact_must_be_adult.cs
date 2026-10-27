using ClassManager.Core.Domain.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_an_account_under_18_accepts_a_contact_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_contact_must_be_adult(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_contact_must_be_adult_Run()
    {
        var fifteenYearsAgo = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime).AddYears(-15);
        var teenagerEmail = StudentAppRequests.UniqueStudentEmail();
        var firstBusiness = await fixture.SeedBusinessAsync();
        var firstFamily = await firstBusiness.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        (await firstBusiness.HttpClient.PostChildAppInvitationAsync(firstFamily.Id, firstFamily.Students.Single().Id, teenagerEmail, fifteenYearsAgo))
            .EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();
        (await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(teenagerEmail)))
            .EnsureSuccessStatusCode();
        var secondBusiness = await fixture.SeedBusinessAsync();
        var secondFamily = await secondBusiness.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        (await secondBusiness.HttpClient.PostStudentAppInvitationAsync(secondFamily.Id, teenagerEmail)).EnsureSuccessStatusCode();

        using var response = await anonymous.PostAcceptStudentAppInvitationAsync(
            fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(teenagerEmail));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(ClientErrorCodes.ContactMustBeAdult);
    }
}
