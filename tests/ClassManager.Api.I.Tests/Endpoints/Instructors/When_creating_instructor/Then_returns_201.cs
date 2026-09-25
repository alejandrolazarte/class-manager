using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_creating_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_201(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_201_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostInstructorAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var instructor = await response.Content.ReadFromJsonAsync<InstructorResponse>(ApiRequests.JsonOptions);
        response.Headers.Location!.ToString().ShouldBe($"{ApiRoutes.Instructors}/{instructor!.Id}");
    }
}
