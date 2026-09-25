using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Students.When_adding_student_to_existing_client;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_201_with_location(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_201_with_location_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();

        using var response = await business.HttpClient.PostStudentAsync(client.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var student = await response.Content.ReadFromJsonAsync<StudentResponse>(ApiRequests.JsonOptions);
        student!.ClientId.ShouldBe(client.Id);
        response.Headers.Location!.ToString().ShouldBe($"{ApiRoutes.Students}/{student.Id}");
    }
}
