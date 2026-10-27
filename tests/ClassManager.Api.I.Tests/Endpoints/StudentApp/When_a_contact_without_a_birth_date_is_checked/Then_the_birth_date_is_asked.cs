using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_contact_without_a_birth_date_is_checked;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_birth_date_is_asked(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_birth_date_is_asked_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        var contactEmail = StudentAppRequests.UniqueStudentEmail();
        (await business.HttpClient.PostStudentAppInvitationAsync(family.Id, contactEmail)).EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostCheckStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(contactEmail));

        (await response.Content.ReadFromJsonAsync<CheckStudentAppInvitationResponse>(ApiRequests.JsonOptions))!.AsksBirthDate.ShouldBeTrue();
    }
}
