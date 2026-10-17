namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_signs_in_to_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_token_carries_their_user(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_token_carries_their_user_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        var businessId = scenario.Instructors.Business.Business.Id;
        var userId = await fixture.FindUserIdByAccountAsync(businessId, UserRows.AccountIdOf(scenario.Tokens.AccessToken));
        UserRows.UserIdOf(scenario.Tokens.AccessToken).ShouldBe(userId!.Value);
    }
}
