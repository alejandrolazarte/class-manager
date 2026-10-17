using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_the_contacts_user_has_other_details;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_client_shows_the_users_details(ApiFixture fixture)
{
    private const string UserFullName = "Ana María Pérez";
    private const string UserEmail = "ana.maria@example.com";
    private const string UserPhoneNumber = "+541155550101";
    private const string StudentUserFullName = "Tomás Andrés Pérez";

    [Fact]
    public async Task Then_the_client_shows_the_users_details_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var registeredClient = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        await fixture.ChangeUserOfClientAsync(business.Business.Id, registeredClient.Id, UserFullName, UserEmail, UserPhoneNumber);
        await fixture.ChangeUserOfStudentAsync(business.Business.Id, registeredClient.Students.Single().Id, StudentUserFullName);

        var client = await business.HttpClient.GetFromJsonAsync<ClientDetailsResponse>(
            new Uri($"{ApiRoutes.Clients}/{registeredClient.Id}", UriKind.Relative), ApiRequests.JsonOptions);

        (client!.FullName, client.Email, client.PhoneNumber, client.Students.Single().FullName)
            .ShouldBe((UserFullName, UserEmail, UserPhoneNumber, StudentUserFullName));
    }
}
