using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_contact_who_attends_accepts_with_an_existing_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_their_student_gets_the_account_birth_date(ApiFixture fixture)
{
    [Fact]
    public async Task Then_their_student_gets_the_account_birth_date_Run()
    {
        var contactEmail = StudentAppRequests.UniqueStudentEmail();
        var firstBusiness = await fixture.SeedBusinessAsync();
        var firstFamily = await firstBusiness.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        (await firstBusiness.HttpClient.PostChildAppInvitationAsync(
            firstFamily.Id, firstFamily.Students.Single().Id, contactEmail, AuthenticationRequests.AdultBirthDate)).EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();
        (await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(contactEmail)))
            .EnsureSuccessStatusCode();
        var secondBusiness = await fixture.SeedBusinessAsync();
        var client = await secondBusiness.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.ClientFullName, null, null)]);
        (await secondBusiness.HttpClient.PostStudentAppInvitationAsync(client.Id, contactEmail)).EnsureSuccessStatusCode();

        (await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(contactEmail)))
            .EnsureSuccessStatusCode();

        var clientDetails = await secondBusiness.HttpClient.GetClientAsync(client.Id);
        clientDetails.Students.Single().BirthDate.ShouldBe(AuthenticationRequests.AdultBirthDate);
    }
}
