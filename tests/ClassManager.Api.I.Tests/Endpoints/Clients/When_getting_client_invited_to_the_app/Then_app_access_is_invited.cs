using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_getting_client_invited_to_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_app_access_is_invited(ApiFixture fixture)
{
    [Fact]
    public async Task Then_app_access_is_invited_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var registeredClient = await business.HttpClient.RegisterClientAsync();
        var email = StudentAppRequests.UniqueStudentEmail();
        using (var invitation = await business.HttpClient.PostStudentAppInvitationAsync(registeredClient.Id, email))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        var client = await business.HttpClient.GetFromJsonAsync<ClientDetailsResponse>(
            new Uri($"{ApiRoutes.Clients}/{registeredClient.Id}", UriKind.Relative), ApiRequests.JsonOptions);

        client!.AppAccess.ShouldBe(new StudentAppAccessResponse(StudentAppAccessStatus.Invited, email));
    }
}
