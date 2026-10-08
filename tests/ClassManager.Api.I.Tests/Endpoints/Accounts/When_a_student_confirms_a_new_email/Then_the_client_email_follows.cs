using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_a_student_confirms_a_new_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_client_email_follows(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_client_email_follows_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var newEmail = EmailChangeRequests.UniqueNewEmail();
        using (var request = await scenario.Student.PostEmailChangeAsync(newEmail, StudentAppRequests.StudentAppPassword))
        {
            request.StatusCode.ShouldBe(HttpStatusCode.Accepted);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var confirmation = await anonymous.PostConfirmEmailChangeAsync(fixture.ApiFactory.EmailTransport.EmailChangeTokenSentTo(newEmail));

        confirmation.StatusCode.ShouldBe(HttpStatusCode.OK);
        var client = await scenario.Instructors.Business.HttpClient.GetFromJsonAsync<ClientDetailsResponse>(
            new Uri($"{ApiRoutes.Clients}/{scenario.ClientId}", UriKind.Relative), ApiRequests.JsonOptions);
        client!.Email.ShouldBe(newEmail);
        using var signIn = await anonymous.PostSignInAsync(newEmail, StudentAppRequests.StudentAppPassword);
        signIn.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
