using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_contact_who_attends_accepts_the_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_their_student_gets_the_account_birth_date(ApiFixture fixture)
{
    [Fact]
    public async Task Then_their_student_gets_the_account_birth_date_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.ClientFullName, null, null)]);
        var contactEmail = StudentAppRequests.UniqueStudentEmail();
        (await business.HttpClient.PostStudentAppInvitationAsync(client.Id, contactEmail)).EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();

        (await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(contactEmail)))
            .EnsureSuccessStatusCode();

        var clientDetails = await business.HttpClient.GetFromJsonAsync<ClientDetailsResponse>(
            new Uri($"{ApiRoutes.Clients}/{client.Id}", UriKind.Relative), ApiRequests.JsonOptions);
        clientDetails!.Students.Single().BirthDate.ShouldBe(AuthenticationRequests.AdultBirthDate);
    }
}
