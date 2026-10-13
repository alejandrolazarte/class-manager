using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_accepts_without_accepting_the_legal_terms;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        var email = StudentAppRequests.UniqueStudentEmail();
        (await business.HttpClient.PostStudentAppInvitationAsync(family.Id, email)).EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostAsJsonAsync(
            ApiRoutes.Authentication + ApiRoutes.AcceptStudentAppInvitation,
            new AcceptStudentAppInvitationCommand(
                fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(email),
                StudentAppRequests.StudentAppFullName,
                StudentAppRequests.StudentAppPassword,
                AuthenticationRequests.AdultBirthDate),
            ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(AuthenticationErrorCodes.LegalTermsNotAccepted);
    }
}
