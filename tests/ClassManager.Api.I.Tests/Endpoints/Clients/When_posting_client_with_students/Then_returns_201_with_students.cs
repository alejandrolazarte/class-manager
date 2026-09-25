using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_posting_client_with_students;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_201_with_students(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_201_with_students_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostClientAsync(
            students: [new NewStudent("Tomás Pérez", new DateOnly(2018, 3, 14), null), new NewStudent("Lucía Pérez", null, null)]);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var client = await response.Content.ReadFromJsonAsync<ClientDetailsResponse>(ApiRequests.JsonOptions);
        client!.Students.Select(student => student.FullName).ShouldBe(["Lucía Pérez", "Tomás Pérez"]);
        var storedClient = await business.HttpClient.GetFromJsonAsync<ClientDetailsResponse>(
            new Uri($"{ApiRoutes.Clients}/{client.Id}", UriKind.Relative), ApiRequests.JsonOptions);
        storedClient!.Students.Count.ShouldBe(2);
    }
}
