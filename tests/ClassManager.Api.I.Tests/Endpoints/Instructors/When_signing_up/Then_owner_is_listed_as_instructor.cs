using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_signing_up;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_owner_is_listed_as_instructor(ApiFixture fixture)
{
    [Fact]
    public async Task Then_owner_is_listed_as_instructor_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        var tokens = await client.SignUpAsync(command);
        using var ownerClient = fixture.CreateClientWithToken(tokens.AccessToken);

        var instructors = await ownerClient.GetFromJsonAsync<List<InstructorResponse>>(
            new Uri(ApiRoutes.Instructors, UriKind.Relative), ApiRequests.JsonOptions);

        instructors!.Select(instructor => instructor.FullName).ShouldBe([command.OwnerFullName]);
    }
}
