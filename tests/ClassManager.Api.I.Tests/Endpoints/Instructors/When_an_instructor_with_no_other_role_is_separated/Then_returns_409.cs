using ClassManager.Core.Domain.Users;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_an_instructor_with_no_other_role_is_separated;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync("Ana Gómez");

        using var response = await business.HttpClient.PostSeparationAsync($"{ApiRoutes.Instructors}/{instructor.Id}", "Martina Gómez");

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(UserErrorCodes.NothingToSeparate);
    }
}
