using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_refreshes_the_session;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_stays_a_student_session(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_stays_a_student_session_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostRefreshAsync(scenario.Tokens.RefreshToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var refreshed = await response.ReadTokensAsync();
        refreshed.Kind.ShouldBe(AccountKinds.Student);
        using var student = fixture.CreateClientWithToken(refreshed.AccessToken);
        (await student.GetStudentAppHomeAsync()).ShouldNotBeNull();
    }
}
