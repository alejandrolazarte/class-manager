using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_getting_client_that_accepted_the_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_app_access_is_active(ApiFixture fixture)
{
    [Fact]
    public async Task Then_app_access_is_active_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        var client = await scenario.Coaches.Business.HttpClient.GetFromJsonAsync<ClientDetailsResponse>(
            new Uri($"{ApiRoutes.Clients}/{scenario.ClientId}", UriKind.Relative), ApiRequests.JsonOptions);

        client!.AppAccess.ShouldBe(new StudentAppAccessResponse(StudentAppAccessStatus.Active, null));
    }
}
