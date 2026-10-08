using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_a_student_switches_back_to_the_owner_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_team_session_is_issued(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_team_session_is_issued_Run()
    {
        var seeded = await fixture.SeedOwnerWhoIsAlsoStudentAsync();
        using var client = fixture.CreateClientWithToken(seeded.OwnerTokens.AccessToken);

        var studentTokens = await (await client.PostSwitchAccountAsync(
            seeded.OwnerTokens.RefreshToken, seeded.Student.Instructors.Business.Business.Id, AccountKinds.Student)).ReadTokensAsync();
        using var studentClient = fixture.CreateClientWithToken(studentTokens.AccessToken);

        using var response = await studentClient.PostSwitchAccountAsync(studentTokens.RefreshToken, seeded.OwnBusinessId, AccountKinds.Team);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.ReadTokensAsync()).Kind.ShouldBe(AccountKinds.Team);
    }
}
