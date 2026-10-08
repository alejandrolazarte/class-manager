using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Api.I.Tests.Endpoints.Roles.When_instructor_creates_a_role;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Instructor.PostRoleAsync(RoleRequests.CustomRoleName, [Permissions.Business.View]);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
