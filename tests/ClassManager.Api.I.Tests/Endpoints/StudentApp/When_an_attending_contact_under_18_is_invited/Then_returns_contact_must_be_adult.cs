using ClassManager.Core.Domain.Clients;
using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.StudentApp;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_an_attending_contact_under_18_is_invited;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_contact_must_be_adult(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_contact_must_be_adult_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        using var registration = await business.HttpClient.PostAsJsonAsync(
            ApiRoutes.Clients,
            new RegisterClientCommand(ApiRequests.ClientFullName, "11 4321-8765", null, null, [new NewStudent(ApiRequests.ClientFullName, null, null)]),
            ApiRequests.JsonOptions);
        var client = (await registration.Content.ReadFromJsonAsync<ClientDetailsResponse>(ApiRequests.JsonOptions))!;

        using var response = await business.HttpClient.PostAsJsonAsync(
            $"{ApiRoutes.Clients}/{client.Id}{ApiRoutes.AppInvitation}",
            new InviteStudentAppRequest(StudentAppRequests.UniqueStudentEmail(), BirthDate: EnrollmentRequests.Today.AddYears(-15)),
            ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(ClientErrorCodes.ContactMustBeAdult);
    }
}
