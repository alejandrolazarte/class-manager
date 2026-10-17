namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_a_student_deletes_their_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_can_no_longer_sign_in(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_can_no_longer_sign_in_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        await scenario.DeleteAccountAsync();

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var signIn = await anonymous.PostSignInAsync(scenario.Email, StudentAppRequests.StudentAppPassword);
        signIn.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        using var refresh = await anonymous.PostRefreshAsync(scenario.Tokens.RefreshToken);
        refresh.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
