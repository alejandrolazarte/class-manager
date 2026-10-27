using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Students.When_a_student_who_uses_the_app_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_signing_in_reports_no_business_access(ApiFixture fixture)
{
    private static readonly DateOnly AdultBirthDate = new(1995, 4, 2);

    [Fact]
    public async Task Then_signing_in_reports_no_business_access_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var client = await owner.RegisterClientAsync();
        using var added = await owner.PostAsJsonAsync(
            $"{ApiRoutes.Clients}/{client.Id}{ApiRoutes.StudentsOfClient}",
            new NewStudent("Martina Pérez", AdultBirthDate, null),
            ApiRequests.JsonOptions);
        var student = (await added.Content.ReadFromJsonAsync<StudentResponse>(ApiRequests.JsonOptions))!;
        var email = StudentAppRequests.UniqueStudentEmail();
        (await owner.PostChildAppInvitationAsync(client.Id, student.Id, email, AdultBirthDate)).EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();
        (await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(email))).EnsureSuccessStatusCode();
        (await owner.DeleteStudentAsync(student.Id)).EnsureSuccessStatusCode();

        using var response = await anonymous.PostSignInAsync(email, StudentAppRequests.StudentAppPassword);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(AuthenticationErrorCodes.NoBusinessAccess);
    }
}
