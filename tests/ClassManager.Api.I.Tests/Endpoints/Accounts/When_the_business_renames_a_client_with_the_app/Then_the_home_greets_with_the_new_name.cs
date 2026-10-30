using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_the_business_renames_a_client_with_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_home_greets_with_the_new_name(ApiFixture fixture)
{
    private const string NewFullName = "Sofía Herrera";

    [Fact]
    public async Task Then_the_home_greets_with_the_new_name_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var business = scenario.Instructors.Business.HttpClient;
        var client = await business.GetClientAsync(scenario.ClientId);
        using (var renamed = await business.PutClientAsync(
            scenario.ClientId, new UpdateClientRequest(NewFullName, client.PhoneNumber, client.Email, client.Notes)))
        {
            renamed.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        var home = await scenario.Student.GetStudentAppHomeAsync();

        home!.SignedInFullName.ShouldBe(NewFullName);
    }
}
